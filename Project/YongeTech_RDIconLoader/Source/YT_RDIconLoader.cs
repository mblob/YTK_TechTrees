﻿using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using KSP;
using RUI.Icons.Simple;

namespace YongeTechKerbal
{
    public class YT_IconData
    {
        public string name;
        public string textureURL;

        public YT_IconData(string name, string textureURL)
        {
            this.name = name;
            this.textureURL = textureURL;
        }
    }

    [KSPAddon(KSPAddon.Startup.SpaceCentre, true)]
    public class YT_RDIconLoader : MonoBehaviour
    {
        string RDIconFolder_name;

        List<YT_IconData> m_iconDataList;

        /************************************************************************\
         * YT_RDIconLoader class                                                *
         * Awake function                                                       *
         *                                                                      *
        \************************************************************************/
        public void Awake()
        {
#if DEBUG
            Debug.Log("YT_RDIconLoader.Awake");
#endif
            DontDestroyOnLoad(this);

            RDIconFolder_name = null;
            m_iconDataList = new List<YT_IconData>();
        }


        /************************************************************************\
         * YT_RDIconLoader class                                                *
         * Start function                                                       *
         *                                                                      *
        \************************************************************************/
        public void Start()
        {
#if DEBUG
            Debug.Log("YT_RDIconLoader.Start");
#endif
            ReadConfigFile();

            //Traverse through GameDatabase looking for folder named RDIconFolder_name
            foreach(UrlDir dir in GameDatabase.Instance.root.AllDirectories)
            {
                if (RDIconFolder_name == dir.name)
                {
                    //Go through all files in this folder looking for .png, .tga, and .dds files
                    //add info for these files to m_iconDataList
                    foreach (UrlDir.UrlFile file in dir.files)
                    {
#if DEBUG
                        Debug.Log("YT_RDIconLoader.Start: looking at file " + file.name + "." + file.fileExtension);
#endif
                        if ("png" == file.fileExtension || "tga" == file.fileExtension || "dds" == file.fileExtension)
                        {
#if DEBUG
                            Debug.Log("YT_RDIconLoader.Start: adding file to iconDataList");
#endif
                            m_iconDataList.Add(new YT_IconData(file.name, file.url));
                        }
                    }
                }
            }
        }


        /************************************************************************\
         * YT_RDIconLoader class                                                *
         * ReadConfigFile function                                              *
         *                                                                      *
         * Reads settings in from the mod's configuration file.                 *
        \************************************************************************/
        private void ReadConfigFile()
        {
#if DEBUG
            Debug.Log("YT_RDIconLoader.ReadConfigFile");
#endif
            //Read Mod Configuration File
            KSP.IO.PluginConfiguration configFile = KSP.IO.PluginConfiguration.CreateForType<YT_RDIconLoader>();
            configFile.load();

            RDIconFolder_name = configFile.GetValue<string>("RDIconFolder_name");
#if DEBUG
            string values = "";
            values += "RDIconFolder = " + RDIconFolder_name + "\n";
            Debug.Log("YT_RDIconLoader.ReadConfigFile: values\n" + values);
#endif
        }


        /************************************************************************\
         * YT_RDIconLoader class                                                *
         * Update function                                                      *
         *                                                                      *
        \************************************************************************/
        public void Update()
        {
#if DEBUG_UPDATE
            Debug.Log("YT_RDIconLoader.Update()");
#endif
            IconLoader iconLoader = FindObjectOfType<IconLoader>();
            if (null != iconLoader)
            {
                LoadIcons(iconLoader);
            }
        }


        /************************************************************************\
         * YT_RDIconLoader class                                                *
         * LoadIcons function                                                   *
         *                                                                      *
        \************************************************************************/
        private void LoadIcons(IconLoader iconLoader)
        {
#if DEBUG
            Debug.Log("YT_RDIconLoader.LoadIcons");
#endif
            Icon customIcon;


            foreach (YT_IconData iconData in m_iconDataList)
            {
#if DEBUG
                Debug.Log("YT_RDIconLoader.LoadIcons: loading " + iconData.name);
#endif
                if (!iconLoader.iconDictionary.ContainsKey(iconData.name))
                {
                    try
                    {
                        customIcon = new Icon("", GameDatabase.Instance.GetTextureInfo(iconData.textureURL).texture);
                        iconLoader.iconDictionary.Add(iconData.name, customIcon);
                        //iconLoader.icons.Add(customIcon);  //it does not appear to be nessicary to add the icon to the list
                    }

                    catch (ArgumentException)
                    {
#if DEBUG
                        Debug.Log("YT_RDIconLoader.LoadIcons: ERROR icon with the name " + iconData.name + "already exisits.");
#endif
                    }
                    catch (NullReferenceException)
                    {
                        Debug.Log("YT_RDIconLoader.LoadIcons: ERROR unable to get texture info for " + iconData.textureURL);
                    }
                }
            }
        }
    }
}
