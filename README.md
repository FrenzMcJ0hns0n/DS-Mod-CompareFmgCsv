# DS-Mod-CompareFmgCsv

## **What ?**

Use this program to compare texts between modded and Vanilla versions of the Souls games.
It takes CSV files as input, and gives bigger CSV files as output with comparisons of values.
See this screenshot as preview :

[TODO : Add image]

Tested and compatible with the texts of Dark Souls Remastered & Demon's Souls. I did not check but it should work with DS:PtdE and DS3 as well.

## **How ?**

The program is written in C#, and hopefully generic enough to be adapted to the text contents of all mods.
Download the .exe file and reproduce directories structure like the following :

```
.
│   DS Mod CompareFmgCsv.exe
│   [log.txt]
│
├───Input
│   ├───Item
│   │   │   filelist.txt
│   │   │
│   │   ├───Mod
│   │   │       ArmorNames.fmg.csv
│   │   │       RingNames.fmg.csv
│   │   │       WeaponNames.fmg.csv
│   │   │       ...
│   │   │
│   │   └───Vanilla
│   │           Armor_name_.fmg.csv
│   │           Accessory_name_.fmg.csv
│   │           Weapon_name_.fmg.csv
│   │           ...
│   │
│   └───Menu
│       │   filelist.txt
│       │
│       ├───Mod
│       │       Conversations.fmg.csv
│       │       EventTexts.fmg.csv
│       │       MovieSubtitles.fmg.csv
│       │       ...
│       │
│       └───Vanilla
│               Conversation_.fmg.csv
│               Event_text_.fmg.csv
│               Movie_subtitles_.fmg.csv
│               ...
│
└───Output
    ├───Item
    └───Menu
```

This app tree shows real filenames of (Mod) Daughter of Ash and (Vanilla) Dark Souls : Remastered.

In the files called ```filelist.txt``` you need, for each msg category (i.e. Item and Menu), to specify the CSV files to compare against each other. Mod on the left, Vanilla on the right, and separate these with a comma. 

According to our app tree, the expected contents are the following :

```./Input/Item/filelist.txt```
```
ArmorNames.fmg.csv	,	Armor_name_.fmg.csv
RingNames.fmg.csv	,	Accessory_name_.fmg.csv
WeaponNames.fmg.csv	,	Weapon_name_.fmg.csv
```

```./Input/Menu/filelist.txt```
```
Conversations.fmg.csv	,	Conversation_.fmg.csv
EventTexts.fmg.csv      ,	Event_text_.fmg.csv
MovieSubtitles.fmg.csv	,	Movie_subtitles_.fmg.csv
```

CSV output files will be created (in ./Output/[Item|Menu] directories), having the same filenames as the Mod input files.
Those output files will expose the data provided in both input files, with all the text IDs from one and the other, and comparisons of their values.

Let's take the first pair of files registered in our example :

```./Input/Item/Mod/ArmorNames.fmg.csv```
```
TextID 1|Value
TextID 2|Value
...
```

```./Input/Item/Vanilla/Armor_name_.fmg.csv```
```
TextID 1|Value
TextID 2|Value
...
```

```./Output/Item/ArmorNames.fmg.csv```
```
Filename|Text ID|Vanilla text|Mod text|Same?
ArmorNames|TextID 1|Vanilla value|Mod value|true
ArmorNames|TextID 2|Vanilla value|Mod value|false
...
```

(yes, all this must become clearer to use)

## **Why ?**

I am making this as I work on the French translation of DoA.

The spreasheet provided as reference was not totally up-to-date, so I wanted to get back a clean work base, not to lose tracks on my progress.

## **How to use it**

1) Use Wulf's BND Rebuilder (or equivalent) to extract the FMG files from .dcx achives
2) Convert input text files from FMG format to CSV (BND Rebuilder bis)
3) Copy these within the directories structure of this project
4) Declare these in pairs within the files "filelist.txt"
5) Run the program
6) Get the output CSV files from the /Output/ subfolders
