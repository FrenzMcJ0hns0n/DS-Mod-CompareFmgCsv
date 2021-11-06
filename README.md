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
│   │   │       Armors.csv
│   │   │       Rings.csv
│   │   │       Weapons.csv
│   │   │       ...
│   │   │
│   │   └───Vanilla
│   │           Armors.csv
│   │           Rings.csv
│   │           Weapons.csv
│   │           ...
│   │
│   └───Menu
│       │   filelist.txt
│       │
│       ├───Mod
│       │       Conversations.csv
│       │       Events.csv
│       │       Movie subtitles.csv
│       │       ...
│       │
│       └───Vanilla
│               Conversations.csv
│               Events.csv
│               Movie subtitles.csv
│               ...
│
└───Output
    ├───Item
    └───Menu
```

In files called filelist.txt, you need to specify the pairs used for comparisons. You need to edit the values yourself, especially if the filenames used in a mod are differents than Vanilla ones (like in Daughter of Ash for instance).

## **Why ?**

I made this as I was working on the French translation of DoA.
The spreasheet provided as reference was not totally up-to-date.
I wanted to get back a clean work base, not to lose tracks on my progress.

## **How to use it**

1) Use Wulf's BND Rebuilder (or equivalent) to extract the FMG files from their .dcx achive
2) Convert input text files from FMG format to CSV (BND Rebuilder bis)
3) Declare the input CSV files in pairs by editing files "filelist.tx"
4) Run the program
5) Get the output CSV files in the /Output/ subfolders
6) Load and beautify the data in a spreadsheet software to obtain something nice to share :)
