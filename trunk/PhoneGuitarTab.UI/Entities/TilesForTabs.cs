﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Phone.Shell;
using PhoneGuitarTab.Data;

namespace PhoneGuitarTab.UI.Entities
{
   public static class TilesForTabs
    {
        private static FlipTileData GetSecondaryTileData(Tab tab)
        {

            if (tab.AlbumCoverImageUrl == "")
                tab.AlbumCoverImageUrl = tab.Group.ImageUrl;
           
            FlipTileData tileData = new FlipTileData
            {
                Title = tab.Name,
                BackgroundImage = new Uri(tab.AlbumCoverImageUrl, UriKind.RelativeOrAbsolute),
                Count = 0,
                BackTitle = tab.TabType.Name,
                BackBackgroundImage = new Uri(GetTabTypeTileImageUrl(tab.TabType.Name), UriKind.Relative),
                BackContent = tab.Name
            };
            
            return tileData;
        }

        public static bool FindTile(string tileUri)
        {
            ShellTile shellTile = ShellTile.ActiveTiles.FirstOrDefault(
                tile => tile.NavigationUri.ToString().Contains(tileUri));
            if (shellTile == null)
                return false;
            else
                return true;
        }

        private static string GetTabTypeTileImageUrl(string tabType)
        {
            //ToDo assign TabType Image Urls 
            switch (tabType)
            {
                case "tab":
                    return "/Images/instrument/guitar_tile.png";
                case "bass":
                    return "/Images/instrument/bass_tile.png";
                case "chords":
                    return "/Images/instrument/chords_tile.png";
                case "drums":
                    return "/Images/instrument/drums_tile.png";
                case Strings.GuitarPro:
                    return "/Images/instrument/guitarpro_tile.png";
                case Strings.MusicXml:
                    return "/Images/instrument/musicxml_tile.png";
                default:
                    return "/Images/instrument/default.png";
            }



        }

        public static void PinTabToStart(Tab tab)
        {
            bool tileExists = FindTile(tab.TabType.TileUrl + tab.Id.ToString());

            if (!tileExists)
            {
                StandardTileData tileData = GetSecondaryTileData(tab);
                ShellTile.Create(new Uri(tab.TabType.TileUrl + tab.Id, UriKind.Relative), tileData, false);
            }        
        }
    }
}
