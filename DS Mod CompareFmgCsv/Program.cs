using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace DS_Mod_CompareFmgCsv
{
    class Program
    {
        // ---------------
        // Manage settings
        // ---------------
        // TODO? Read from external file
        readonly static string[] msgcategories = { "Item", "Menu" };




        static void Main(string[] args)
        {
            // ------------------
            // Verify directories
            // ------------------
            try
            {
                string root_dir = ReturnRootDir();

                string input_item_dir = ReturnInputCategoryDir("Item");
                string input_menu_dir = ReturnInputCategoryDir("Menu");

                string input_item_filelist_file = ReturnInputCatFilelistFile("Item");
                string input_menu_filelist_file = ReturnInputCatFilelistFile("Menu");

                string input_item_mod_dir = ReturnInputCategoryModDir("Item");
                string input_menu_mod_dir = ReturnInputCategoryModDir("Menu");

                string input_item_van_dir = ReturnInputCategoryVanDir("Item");
                string input_menu_van_dir = ReturnInputCategoryVanDir("Menu");

                string item_output_dir = ReturnOutputCategoryDir("Item");
                string menu_output_dir = ReturnOutputCategoryDir("Menu");

                Console.WriteLine("Directories verified.");
            }
            catch (Exception ex)
            {
                LogMessage("Failed to verify directories. Exception :\n" + ex.ToString());
                Environment.Exit(1);
            }

            // ---------------------
            // Check input CSV files
            // ---------------------
            try
            {
                bool missing_files = false;
                foreach (string msgcat in msgcategories)
                {
                    Dictionary<string, string> filenames_dictionary = ReturnDictionaryFromInputFile(msgcat);
                    string mod_directory = ReturnInputCategoryModDir(msgcat);
                    string van_directory = ReturnInputCategoryVanDir(msgcat);

                    foreach (KeyValuePair<string, string> fd in filenames_dictionary)
                    {
                        string mod_filepath = Path.Combine(mod_directory, fd.Key);
                        if (!File.Exists(mod_filepath))
                        {
                            LogMessage($"Error: Missing file '{fd.Key}' in {msgcat}/Mod directory\n'{mod_directory}'");
                            missing_files = true;
                        }

                        string van_filepath = Path.Combine(van_directory, fd.Value);
                        if (!File.Exists(van_filepath))
                        {
                            LogMessage($"Error: Missing file '{fd.Value}' in {msgcat}/Vanilla directory\n'{van_directory}'");
                            missing_files = true;
                        }
                    }
                }

                if (missing_files) Environment.Exit(1);
                Console.WriteLine("Files checked.");
            }
            catch (Exception ex)
            {
                LogMessage("Failed to check files. Exception :\n" + ex.ToString());
                Environment.Exit(1);
            }


            // ----------------------
            // Build output CSV files
            // ----------------------
            Console.WriteLine("\nProcessing files...");
            ProcessFiles("Item");
            ProcessFiles("Menu");

            Console.WriteLine("CSV output files created.");
            Console.WriteLine("\nDONE. PRESS ANY KEY TO EXIT");
            Console.ReadKey();
            Environment.Exit(0);
        }




        /// <summary>
        /// Get the filenames to work on.
        /// Return them as a Dictionary.
        /// </summary>
        /// <param name="msgcat">"Item" or "Menu"</param>
        static Dictionary<string, string> ReturnDictionaryFromInputFile(string msgcat)
        {
            Dictionary<string, string> filenames_dictionary = new Dictionary<string, string>();
            string[] files_lines = File.ReadAllLines(ReturnInputCatFilelistFile(msgcat));
            foreach (string line in files_lines)
            {
                string mod_filename = line.Split(',')[0].Trim();
                string van_filename = line.Split(',')[1].Trim();

                if (mod_filename == "") continue; // 'van_filename' can be empty (meaning Vanilla filename = Mod filename), but 'mod_filename' cannot
                filenames_dictionary.Add(mod_filename, van_filename);
            }
            return filenames_dictionary;
        }




        /// <summary>
        /// Process input CSV files for the selected msg category.
        /// Produce bigger output CSV files with both Mod & Vanilla data.
        /// </summary>
        /// <param name="msgcat">"Item" or "Menu"</param>
        static void ProcessFiles(string msgcat)
        {
            try
            {
                Dictionary<string, string> category_dictionnary = ReturnDictionaryFromInputFile(msgcat);
                string mod_dir = ReturnInputCategoryModDir(msgcat);
                string van_dir = ReturnInputCategoryVanDir(msgcat);

                foreach (KeyValuePair<string, string> cd in category_dictionnary)
                {
                    // Set lists of lines we can loop on
                    string[] mod_lines = File.ReadAllLines(Path.Combine(mod_dir, cd.Key));
                    string[] van_lines = File.ReadAllLines(Path.Combine(van_dir, cd.Value));

                    // Prepare output elements
                    string mod_filename = Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(cd.Key));
                    string output_filepath = Path.Combine(ReturnOutputCategoryDir(msgcat), cd.Key);

                    // Fill sorted dictionary with modded values first
                    SortedDictionary<int, string> output_dictionary = new SortedDictionary<int, string>();
                    foreach (string line in mod_lines)
                    {
                        string str_id = line.Split('|')[0].Trim();
                        string modval = line.Split('|')[1].Trim();

                        if (modval == "") continue; // Exclude empty modded values

                        if (int.TryParse(str_id, out int id))
                        {
                            output_dictionary.Add(id, modval);
                        }
                    }

                    // Then, add Vanilla values and compare them against modded ones
                    foreach (string line in van_lines)
                    {
                        string str_id = line.Split('|')[0].Trim();
                        string vanval = line.Split('|')[1].Trim();

                        if (vanval == "") continue; // Exclude empty Vanilla values

                        if (int.TryParse(str_id, out int id))
                        {
                            if (output_dictionary.ContainsKey(id)) // [Mod IDs list] contains [Vanilla ID] = both Vanilla & modded values that we can compare
                            {
                                if (output_dictionary.TryGetValue(id, out string modval))
                                {
                                    output_dictionary[id] = string.Format("{0}|{1}|{2}|{3}|{4}", mod_filename, id, vanval, modval, (vanval == modval) ? "true" : "false");
                                }
                            }
                            else // [Mod IDs list] does NOT contain [Vanilla ID] = Vanilla value only
                            {
                                output_dictionary.Add(id, string.Format("{0}|{1}|{2}||false", mod_filename, id, vanval));
                            }
                        }
                    }

                    // Also get back on formatting lines where [Mod IDs list] does NOT contain [Vanilla ID] : modded value only
                    SortedDictionary<int, string> output_dictionary_replica = new SortedDictionary<int, string>(output_dictionary);
                    foreach (KeyValuePair<int, string> odr in output_dictionary_replica)
                    {
                        if (!odr.Value.Contains("|true") && !odr.Value.Contains("|false"))
                        {
                            if (output_dictionary_replica.TryGetValue(odr.Key, out string value))
                            {
                                output_dictionary[odr.Key] = string.Format("{0}|{1}||{2}|false", mod_filename, odr.Key, odr.Value);
                            }
                        }
                    }

                    // Write output_data to CSV file
                    using (StreamWriter writer = new StreamWriter(output_filepath, false))
                    {
                        writer.WriteLine($"Filename|Text ID|Vanilla text|Mod text|Same?");
                        foreach (KeyValuePair<int, string> od in output_dictionary)
                        {
                            writer.WriteLine(od.Value);
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                LogMessage("Failed to process files. Exception :\n" + ex.ToString());
                Environment.Exit(1);
            }
        }




        #region "Harcoded dictionaries of filenames: [DSR - ENGLISH] Daughter of Ash 1.5.2 vs. Vanilla"
        static Dictionary<string, string> ReturnItemDictionary_DSR_DoA()
        {
            return new Dictionary<string, string>()
            {   
                // DoA filenames                ,  Vanilla filenames
                { "ArmorDescriptions.fmg.csv"   ,  "Armor_long_desc_.fmg.csv"       },
                { "ArmorNames.fmg.csv"          ,  "Armor_name_.fmg.csv"            },
                { "ArmorSummaries.fmg.csv"      ,  "Armor_description_.fmg.csv"     }, // no use in DoA
                { "FeatureDescriptions.fmg.csv" ,  "Feature_long_desc_.fmg.csv"     }, // no use in DoA
                { "FeatureNames.fmg.csv"        ,  "Feature_name_.fmg.csv"          }, // no use in DoA
                { "FeatureSummaries.fmg.csv"    ,  "Feature_description_.fmg.csv"   }, // no use in DoA
                { "ItemDescriptions.fmg.csv"    ,  "Item_long_desc_.fmg.csv"        },
                { "ItemNames.fmg.csv"           ,  "Item_name_.fmg.csv"             },
                { "ItemSummaries.fmg.csv"       ,  "Item_description_.fmg.csv"      },
                { "MagicDescriptions.fmg.csv"   ,  "Magic_long_desc_.fmg.csv"       },
                { "MagicNames.fmg.csv"          ,  "Magic_name_.fmg.csv"            },
                { "MagicSummaries.fmg.csv"      ,  "Magic_description_.fmg.csv"     },
                { "NPCNames.fmg.csv"            ,  "NPC_name_.fmg.csv"              },
                { "PlaceNames.fmg.csv"          ,  "Place_name_.fmg.csv"            },
                { "RingDescriptions.fmg.csv"    ,  "Accessory_long_desc_.fmg.csv"   },
                { "RingNames.fmg.csv"           ,  "Accessory_name_.fmg.csv"        },
                { "RingSummaries.fmg.csv"       ,  "Accessory_description_.fmg.csv" },
                { "WeaponDescriptions.fmg.csv"  ,  "Weapon_long_desc_.fmg.csv"      },
                { "WeaponNames.fmg.csv"         ,  "Weapon_name_.fmg.csv"           },
                { "WeaponSummaries.fmg.csv"     ,  "Weapon_description_.fmg.csv"    }
            };
        }
        static Dictionary<string, string> ReturnMenuDictionary_DSR_DoA()
        {
            return new Dictionary<string, string>()
            {   
                // DoA filenames                ,  Vanilla filenames
                { "BloodMessages.fmg.csv"       ,  "Blood_writing_.fmg.csv"                 },
                { "ContextualHelp.fmg.csv"      ,  "Item_help_.fmg.csv"                     }, // no use in DoA
                { "Conversations.fmg.csv"       ,  "Conversation_.fmg.csv"                  }, // CHECK : many more lines in Vanilla
                { "DebugTags_Win32.fmg.csv"     ,  "System_specific_tags_win32_.fmg.csv"    }, // CHECK : no use ?
                { "EventTexts.fmg.csv"          ,  "Event_text_.fmg.csv"                    },
                { "IngameMenus.fmg.csv"         ,  "Ingame_menu_.fmg.csv"                   },
                { "KeyGuide.fmg.csv"            ,  "Key_guide_.fmg.csv"                     },
                { "MenuDialogs.fmg.csv"         ,  "Dialogue_.fmg.csv"                      },
                { "MenuHelpSnippets.fmg.csv"    ,  "Single_line_help_.fmg.csv"              },
                { "MenuText_Common.fmg.csv"     ,  "Menu_general_text_.fmg.csv"             },
                { "MenuText_Other.fmg.csv"      ,  "Menu_others_.fmg.csv"                   },
                { "MovieSubtitles.fmg.csv"      ,  "Movie_subtitles_.fmg.csv"               },
                { "SystemMessages_Win32.fmg.csv",  "System_message_win32_.fmg.csv"          },
                { "TextTagPlaceholders.fmg.csv" ,  "Text_display_tag_list_.fmg.csv"         }  // CHECK : no use ?
            };
        }
        #endregion




        static void LogMessage(string message)
        {
            File.AppendAllText(ReturnLogFilepath(), $"{DateTime.Now} - {message}\n");
        }




        static string ReturnRootDir() { return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location); }
        static string ReturnInputCategoryDir(string msgcat) { return Path.Combine(ReturnRootDir(), "Input", msgcat); }
        static string ReturnInputCatFilelistFile(string msgcat) { return Path.Combine(ReturnRootDir(), "Input", msgcat, "filelist.txt"); }
        static string ReturnInputCategoryModDir(string msgcat) { return Path.Combine(ReturnRootDir(), "Input", msgcat, "Mod"); }
        static string ReturnInputCategoryVanDir(string msgcat) { return Path.Combine(ReturnRootDir(), "Input", msgcat, "Vanilla"); }
        static string ReturnOutputCategoryDir(string msgcat) { return Path.Combine(ReturnRootDir(), "Output", msgcat); }

        //static string ReturnSettingsDirectory()               { return Path.Combine(ReturnRootDirectory(), "Settings"); } //TODO?

        static string ReturnLogFilepath() { return Path.Combine(ReturnRootDir(), "log.txt"); }
    }
}
