using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class AudioCatalog : MonoBehaviour
{
    #region "Types"

    [Serializable]
    public class IndexedClip
    {
        public string Id;
        public AudioClip Clip;
        public override string ToString() { return this.Id; }
    }

    [Serializable]
    public class IndexedClipList
    {
        public string Id;
        public AudioClip[] Clips;
        public bool InmediateCC = false;
        public override string ToString() { return this.Id; }
    }

    [Serializable]
    public class PlaylistDef
    {
        public string Id;
        public string[] Songs;

        [NonSerialized]
        public int Index = 0;

        public override string ToString() { return Id;}
    }

    #endregion

    #region "Configurables"

    public IndexedClipList[] SoundEffects;

    public IndexedClip[] Songs;

    public PlaylistDef[] Playlists;

    #endregion

    #region "Sound Fx"

    private Dictionary<string, IndexedClipList> sfxDictionary;

    private void indexSfx()
    {
        sfxDictionary = new Dictionary<string, IndexedClipList>();
        for (int i=0; i<this.SoundEffects.Length;i++)
        {
            this.sfxDictionary.Add(this.SoundEffects[i].Id, this.SoundEffects[i]);
        }
    }

    public IndexedClipList GetSfx(string Id)
    {
        IndexedClipList retVal = null;
        this.sfxDictionary.TryGetValue(Id, out retVal);
        return retVal;
    }

    #endregion

    #region "Songs"

    private Dictionary<string, IndexedClip> songsDictionary;

    private void indexSongs()
    {
        songsDictionary = new Dictionary<string, IndexedClip>();
        for (int i = 0; i < this.Songs.Length; i++)
        {
            songsDictionary.Add(this.Songs[i].Id, this.Songs[i]);
        }
    }

    public IndexedClip GetSong(string Id)
    {
        IndexedClip retVal = null;
        this.songsDictionary.TryGetValue(Id, out retVal);
        return retVal;
    }

    #endregion

    #region "Playlists"

    private Dictionary<string, PlaylistDef> playlistDictionary;

    private void indexPlaylists()
    {
        playlistDictionary = new Dictionary<string, PlaylistDef>();
        for (int i = 0; i < this.Playlists.Length; i++)
        {
            playlistDictionary.Add(this.Playlists[i].Id, this.Playlists[i]);
        }
    }

    public PlaylistDef GetPlaylist(string Id)
    {
        PlaylistDef retVal = null;
        this.playlistDictionary.TryGetValue(Id, out retVal);
        return retVal;
    }

    #endregion

    /// <summary>
    /// Create the indexes for the Songs, Playlists and SoundEffects
    /// </summary>
    public void Initialize()
    {
        indexPlaylists();
        indexSfx();
        indexSongs();
    }

}
