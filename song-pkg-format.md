Here is how a song in WacK is packaged.

# Song folder Layout
```
<any name>/
├─ 0.mer
├─ 1.mer
├─ 2.mer
├─ 3.mer
├─ meta.mer
├─ jacket.png/.jpg
├─ *.mp3/.ogg/.wav //SONG AUDIO
```

# meta.mer
**This file must be present to define its folder as a song.** Everything here only dictates how the song is displayed in song select; none of these tags affect how the chart is played.

```
#TITLE <str>
#RUBI <str>
#ARTIST <str>
#COPYRIGHT <str?>
#GENRE <str?>
#BPM <str>
#PREVIEW_TIME <float>
#PREVIEW_DURATION <float>
```
*`?` = optional

# Chart .mer
### Naming
**Every charted .mer file must contain an integer that may be padded.** The first four, 0-3, correspond to in-game difficulty as follows:
```
0. Normal
1. Hard
2. Expert
3. Inferno
```

### Tags
`#LEVEL` (float): difficulty level  
`#MUSIC_FILE_PATH` (str)  
`#OFFSET` (float)  
`#CLEAR_THRESHOLD` (float? 0-1)  
`#AUTHOR` (str?)  
`#PREVIEW_TIME` (float? seconds)  
`#PREVIEW_DURATION` (float? seconds)  
`#MOVIE_FILE_PATH` (str?)  
`#MOVIE_OFFSET` (float? seconds)

*`?` = optional
