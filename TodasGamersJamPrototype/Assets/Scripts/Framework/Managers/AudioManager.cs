using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioManager
{
    #region "Initialization"

    public void Initialize()
    {
        //create the audioCatalogs array
        this.audioCatalogs = new AudioCatalog[] { };
    }

    #endregion

    #region "Audio Catalogs"

    private AudioCatalog[] audioCatalogs = null;

    public void RegisterCatalog(AudioCatalog cat)
    {
        //first, look if the audiocatalog exists already
        bool existing = false;
        for (int i=0;i<this.audioCatalogs.Length; i++)
        {
            if (this.audioCatalogs[i] == cat) { existing = true; break; }
        }

        //if doesn't exists, add it
        if (!existing)
        {
            AudioCatalog[] newCatalog = new AudioCatalog[audioCatalogs.Length + 1];
            audioCatalogs.CopyTo(newCatalog, 0);
            newCatalog[audioCatalogs.Length] = cat;
            this.audioCatalogs = newCatalog;
            //initialize the catalog
            cat.Initialize();
        }
    }

    #endregion

    #region "Sounds being played"

    /// <summary>
    /// This class is used to keep track of sounds being played
    /// </summary>
    public class PlayingSound
    {
        public AudioSource Source;
        public bool DestroyOnFinish;
        public bool Looped;
        public GameManager.GameEvent Finished;
        public bool Fading;
        public float FadeTimeLeft = 0.0f;
        public float FadeDuration = 0.0f;
        public float FadeStartVolume = 0.0f;
        public bool InmediateCC = false;
        public string Id;
        public bool recycle = false;
    }

    /// <summary>
    /// List of sounds currently being played
    /// </summary>
    private List<PlayingSound> playingSounds = new List<PlayingSound>();

    #endregion

    #region "Sound Effects"

    /// <summary>
    /// Plays a sound effect
    /// </summary>
    /// <param name="Id">Id of the sound effect in the catalog</param>
    /// <param name="source">AudioSource where the sound will come from. Null to generate a temporary one</param>
    /// <param name="loop">Is the sound looped?</param>
    /// <param name="finished">Specify a method you want to be invoked when the sound finished playing</param>
    public void PlaySfx( string Id, AudioSource source, bool loop, GameManager.GameEvent finished)
    {
        if (!string.IsNullOrEmpty(Id))
        {
            AudioCatalog.IndexedClipList clip = findSfxInCatalogs(Id);
            AudioClip aClip = null;
            if (clip!=null)
            {
                //decide which audio clip to play
                if ((clip.Clips!=null) && (clip.Clips.Length>0))
                {
                    //select a random clip from the ones available
                    aClip = clip.Clips[Random.Range(0, clip.Clips.Length)];
                }
                else
                {
                    Debug.LogError(string.Format("SFX {0} does not contains any AudioClips!", Id));
                }

                bool asCreated = false;
                if (aClip!=null)
                {
                    //if no AudioSource was specified, create a new one
                    if (source == null)
                    {
                        GameObject gO = GetAudioObject(Id);
                        source = gO.GetComponent<AudioSource>();
                        asCreated = true;
                    }

                    //Add the sound to the sounds being played
                    PlayingSound pS = new PlayingSound();
                    pS.Source = source;
                    pS.DestroyOnFinish = asCreated;
                    pS.Looped = loop;
                    pS.Finished = finished;
                    pS.Id = Id;
                    pS.InmediateCC = clip.InmediateCC;
                    pS.recycle = asCreated;
                    this.playingSounds.Add(pS);

                    //TODO: Soporte de Closed Captions

                    //actually play the sound
                    source.clip = aClip;
                    source.loop = loop;
                    source.volume = GameManager.Instance.Options.Options.SfxVolume;
                    source.Play();
                }
            }
            else
            {
                Debug.LogError(string.Format("SFX {0} not defined in any AudioCatalog!", Id));
            }
        }
    }

    /// <summary>
    /// Stops a sound playing with the given Id
    /// </summary>
    public void StopSfx( string Id, float fadeTime )
    {
        //find the sound by its Id
        PlayingSound sToDestroy = null;
        for (int i = 0; i < this.playingSounds.Count; i++)
        {
            PlayingSound s = this.playingSounds[i];
            if ((s.Id == Id) && (!s.Fading))
            {
                sToDestroy = s;
                break;
            }
        }

        //mark it for destruction
        markSfxForDestruction(sToDestroy, fadeTime);
    }

    #endregion

    #region "Music"

    public float FadeOutDuration = 4.0f;
    public float FadeInDuration = 4.0f;

    private AudioCatalog.PlaylistDef currentPlaylistDef;
    private string[] currentPlaylist;
    private string currentSongName = "";
    private bool currentSongRepeat = false;
    private AudioSource songSource = null;

    private string pendingMusic = null;
    private bool pendingLoop = false;

    private float fadeTime = 0.0f;
    private float fadeTimeLeft = 0.0f;
    private bool fadingOut = false;
    private bool fadingIn = false;
    private bool stopMusic = false;
    private bool pauseMusic = false;

    /// <summary>
    /// Plays the Song or Playlist specified
    /// </summary>
    public void PlayMusic(string Id, bool loop)
    {
        if (((this.currentSongName == null) || (Id != this.currentSongName))
            && ((this.currentPlaylistDef == null) || (this.currentPlaylistDef.Id != Id)))
        {
            //is there a song playing?
            if ((this.currentSongName != null) && (this.currentSongName != string.Empty))
            {
                this.pendingMusic = Id;
                this.pendingLoop = loop;

                //start the fade out
                this.fadeTimeLeft = this.FadeOutDuration;
                this.fadeTime = this.FadeOutDuration;
                this.fadingOut = true;
                this.stopMusic = true;
            }
            else
            {
                this.doPlayMusic(Id, loop);
            }
        }
    }

    private void doPlayMusic(string Id, bool loop, bool inmediate = false)
    {
        AudioCatalog.PlaylistDef playlist = this.findPlaylistInCatalogs(Id);
        if (playlist!=null)
        {
            if (currentPlaylistDef != playlist)
            {
                setNewPlaylist(playlist);
                playlistPlayNext();
            }
        }
        else
        {
            //clear current playlist
            currentPlaylistDef = null;
            currentPlaylist = null;

            //try to play the Id as a song
            this.playSong(Id, loop, inmediate);
        }
    }

    /// <summary>
    /// Play a Song
    /// </summary>
    /// <param name="Id">The Id of the Song you want to be played</param>
    /// <param name="loop">Play in loop mode</param>
    /// <param name="inmediate">Don't fade out if true</param>
    private void playSong(string Id, bool loop, bool inmediate = false)
    {
        bool played = false;
        AudioCatalog.IndexedClip song = this.findSongInCatalogs(Id);
        if (song!=null)
        {
            if (song.Clip!=null)
            {
                //make sure we have a audiosource to play the song with
                this.refreshSongSource();

                //play the song
                this.songSource.volume =
                    inmediate ? GameManager.Instance.Options.Options.MusicVolume : 0.0f;
                this.songSource.clip = song.Clip;
                this.songSource.loop = loop;
                this.songSource.Play();

                //fade out the previous song
                if (!inmediate)
                {
                    this.fadeTime = FadeInDuration;
                    this.fadeTimeLeft = FadeInDuration;
                    this.fadingIn = true;
                }
                else
                {
                    this.fadingIn = false;
                }

                //anotate the current song
                this.currentSongName = song.Id;
                this.currentSongRepeat = loop;

                played = true;
                pauseMusic = false;
                stopMusic = false;
            }
        }

        if (!played)
        {
            if ((Id != null) && (Id!=string.Empty))
                Debug.LogError(string.Format("Song {0} not found in AudioCatalogs!", Id));
        }
    }

    /// <summary>
    /// Set a playlist as the current one
    /// </summary>
    private void setNewPlaylist(AudioCatalog.PlaylistDef playlist)
    {
        currentPlaylistDef = playlist;
        currentPlaylist = playlist.Songs.Clone() as string[];

        //make sure index are in range
        if (this.currentPlaylistDef.Index >= currentPlaylist.Length) this.currentPlaylistDef.Index = 0;
    }

    /// <summary>
    /// Plays the next in the playlist, and points to the next one
    /// </summary>
    private void playlistPlayNext()
    {

        //make sure index are in range
        if (this.currentPlaylistDef.Index >= currentPlaylist.Length) this.currentPlaylistDef.Index = 0;
        string nextSong = currentPlaylist[currentPlaylistDef.Index];
        if (nextSong != currentSongName)
        {
            bool played = false;

            //make sure it's available
            if (findSongInCatalogs(nextSong)!=null)
            {
                playSong(nextSong, false);
                //increment the index of the playlist
                this.currentPlaylistDef.Index++;
                played = true;
            }
            else
            {
                AudioCatalog.PlaylistDef playlist = findPlaylistInCatalogs(nextSong);
                if (playlist!=null)
                {
                    setNewPlaylist(playlist);
                    played = true;
                }
            }

            if (!played)
                Debug.LogError(
                    string.Format("Song {0} of Playlist {1} not found!", nextSong, currentPlaylistDef.Id));
        }
    }

    /// <summary>
    /// Stops the music currently being played and suspends the playlist if any
    /// </summary>
    public void StopSong(float fadeTime = 0.0f)
    {
        if ((this.currentSongName != null) && (this.currentSongName != string.Empty))
        {
            pendingMusic = "";
            if (fadeTime > 0.0f)
            {
                this.fadeTime = fadeTime;
                this.fadeTimeLeft = fadeTime;
                this.fadingOut = true;
                this.stopMusic = true;
            }
            else
            {
                //stop and clear
                this.songSource.Stop();
                this.songSource.clip = null;
                //this.songSource = null;
                this.currentSongName = null;
                this.currentPlaylistDef = null;
                this.currentPlaylist = null;
                this.currentSongRepeat = false;
            }
        }
    }

    #endregion

    #region "Update"

    /// <summary>
    /// Update sounds that are currently playing
    /// </summary>
    public void Update()
    {
        //update playing sounds
        this.updatePlayingSounds();

        //update fades and music/plalists
        this.updateMusic();
    }

    /// <summary>
    /// Update the music being played
    /// </summary>
    private void updateMusic()
    {
        //manage finished songs
        if (songSource != null)
        {
            if (!songSource.isPlaying && !pauseMusic)
            {
                this.playSongFinished(stopMusic);
            }
        }

        //manage fade in
        if (this.fadingIn)
        {
            this.fadeTimeLeft -= Time.deltaTime;
            if (this.fadeTimeLeft <= 0.0f)
            {
                this.songSource.volume = GameManager.Instance.Options.Options.MusicVolume;
                this.fadeTime = 0.0f;
                this.fadeTimeLeft = 0.0f;
                this.fadingIn = false;
            }
            else
            {
                this.songSource.volume = ((this.fadeTime-this.fadeTimeLeft) / this.fadeTime)
                    * GameManager.Instance.Options.Options.MusicVolume;
            }
        }

        //manage fade out
        if (this.fadingOut)
        {
            this.fadeTimeLeft -= Time.deltaTime;
            if (this.fadeTimeLeft <= 0.0f)
            {
                this.songSource.volume = 0.0f;
                this.fadeTime = 0.0f;
                this.fadeTimeLeft = 0.0f;
                this.fadingOut = false;

                if (stopMusic)
                {
                    this.songSource.Stop();
                    this.songSource.clip = null;
                    //this.songSource = null;
                    this.currentSongName = "";
                    this.currentSongRepeat = false;

                    //play the song we annotated as the pending one
                    doPlayMusic(this.pendingMusic, this.pendingLoop);

                    //clear pendings
                    this.pendingLoop = false;
                    this.pendingMusic = null;
                    

                    stopMusic = false;
                }
                else
                {
                    this.songSource.Pause();
                }

            }
            else
            {
                this.songSource.volume =
                    (this.fadeTimeLeft / this.fadeTime) * GameManager.Instance.Options.Options.MusicVolume;
            }
        }
    }

    /// <summary>
    /// The song being played has finished, process loop or next playlist song
    /// </summary>
    private void playSongFinished(bool stopMusic)
    {
        if (!stopMusic)
        {
            //is current song in reapeat mode? Just play it again
            if (this.currentSongRepeat)
            {
                this.songSource.Play();
            }
            else
            {
                if (this.currentPlaylist != null)
                {
                    this.playlistPlayNext();
                }
                else
                {
                    this.currentSongName = "";
                }
            }
        }
    }

    /// <summary>
    /// Update the sounds being played
    /// </summary>
    private void updatePlayingSounds()
    {
        for (int i=0;i<this.playingSounds.Count;i++)
        {
            PlayingSound s = this.playingSounds[i];
            if ((s==null)||(s.Source==null))
            {
                this.playingSounds.RemoveAt(i);
            }
            else
            {
                //fading sounds, must be destroyed when the fade is done
                bool justDestroyed = false;
                if (s.Fading)
                {
                    s.FadeTimeLeft -= Time.deltaTime;
                    if (s.FadeTimeLeft<=0.0f)
                    {
                        destroySound(s);
                        this.playingSounds.RemoveAt(i);
                        justDestroyed = true;
                    }
                    else
                    {
                        s.Source.volume = s.FadeStartVolume * (s.FadeTimeLeft / s.FadeDuration);
                    }
                }
                
                //sounds that has finished playing and are loop
                if (!justDestroyed)
                {
                    if ((s.Source==null) || (!s.Source.isPlaying))
                    {
                        if (s.Looped && (s.Source!=null))
                        {
                            s.Source.Play();
                        }
                        else
                        {
                            destroySound(s);
                            this.playingSounds.RemoveAt(i);
                        }
                    }
                }

            }

        }
    }

    #endregion

    #region "Utils"

    /// <summary>
    /// Searchs for a Sfx in all the catalogs currently in memory
    /// </summary>
    private AudioCatalog.IndexedClipList findSfxInCatalogs(string Id)
    {
        AudioCatalog.IndexedClipList retVal = null;
        for (int i = 0; i < this.audioCatalogs.Length; i++)
        {
            AudioCatalog.IndexedClipList c = this.audioCatalogs[i].GetSfx(Id);
            if (c != null) { retVal = c; break; }
        }
        return retVal;
    }

    /// <summary>
    /// Searchs for a Song in all the catalogs currently in memory
    /// </summary>
    private AudioCatalog.IndexedClip findSongInCatalogs(string Id)
    {
        AudioCatalog.IndexedClip retVal = null;
        for (int i = 0; i < this.audioCatalogs.Length; i++)
        {
            AudioCatalog.IndexedClip c = this.audioCatalogs[i].GetSong(Id);
            if (c != null) { retVal = c; break; }
        }
        return retVal;
    }

    /// <summary>
    /// Searchs for a Playlist in all the catalogs currently in memory
    /// </summary>
    private AudioCatalog.PlaylistDef findPlaylistInCatalogs(string Id)
    {
        AudioCatalog.PlaylistDef retVal = null;
        for (int i = 0; i < this.audioCatalogs.Length; i++)
        {
            AudioCatalog.PlaylistDef c = this.audioCatalogs[i].GetPlaylist(Id);
            if (c != null) { retVal = c; break; }
        }
        return retVal;
    }

    /// <summary>
    /// Destroy a sound that was playing
    /// </summary>
    private void destroySound(PlayingSound s)
    {
        if (s.Source!=null)
        {
            s.Source.Stop();
            s.Source.clip = null;
            if (s.DestroyOnFinish)
            {
                if (s.recycle) RecycleAudioObject(s.Source.gameObject);
                s.Source = null;
            }
        }

        //if there is someone interested in know when 
        if (s.Finished != null) s.Finished();
        s.Finished = null;
    }

    /// <summary>
    /// If we haven't yet, look for the AudioSource that will play the songs
    /// </summary>
    private void refreshSongSource()
    {
        if (songSource==null)
        {
            this.songSource =
                GameManager.Instance.Logic.CurrentLevel.GetComponent<AudioSource>();
        }
    }

    public void StopEverything(float fadeTime)
    {
        //stop current playing song
        StopSong(fadeTime);

        //stop all playing sounds
        for (int i=0; i<this.playingSounds.Count; i++)
        {
            markSfxForDestruction(this.playingSounds[i], fadeTime);
        }
    }

    private void markSfxForDestruction(PlayingSound sToDestroy, float fadeTime)
    {
        if (sToDestroy != null)
        {
            sToDestroy.FadeStartVolume = (sToDestroy.Source != null) ? sToDestroy.Source.volume : 0.0f;
            sToDestroy.Fading = true;
            sToDestroy.FadeTimeLeft = fadeTime;
            sToDestroy.FadeDuration = fadeTime;
        }
    }
    #endregion

    #region "Pooling"
    
    /// <summary>
    /// Stores the GameObjects with an audiosource component
    /// </summary>
    public List<GameObject> audioObjPool = new List<GameObject>();

    /// <summary>
    /// Get a game object with an audiosource component from the pool,
    /// or creates a new one if the pool is empty
    /// </summary>
    private GameObject GetAudioObject(string id)
    {
        GameObject ao = null;
        string goName = "Sound: " + id;
        if (audioObjPool.Count > 0)
        {
            ao = audioObjPool[audioObjPool.Count - 1];
            ao.name = goName;
            audioObjPool.Remove(ao);
            ao.SetActive(true);
        }
        else
        {
            ao = new GameObject(goName);
            ao.AddComponent<AudioSource>();
        }
        return ao;
    }

    /// <summary>
    /// Store a game object with an audiosource component
    /// </summary>
    private void RecycleAudioObject(GameObject go)
    {
        audioObjPool.Add(go);
        go.SetActive(false);
    }

    /// <summary>
    /// Drains the audio pool
    /// </summary>
    public void DrainPool()
    {
        if(audioObjPool != null)
        {
            if(audioObjPool.Count > 0) audioObjPool.Clear();
        }
    }

    #endregion

}
