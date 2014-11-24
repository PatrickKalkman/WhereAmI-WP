using Microsoft.Phone.Shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhereAmI.Common
{
    public class FlipTileCreator
    {
        public void CreateTile()
        {
            var navigationUri = CreateNavigationUri();

            FlipTileData tileData = CreateTileData();

            ShellTile tile = ShellTile.ActiveTiles.FirstOrDefault(t => t.NavigationUri == navigationUri);

            if (tile == null)
            {
                ShellTile.Create(navigationUri, tileData, true);
            }
            else
            {
                tile.Update(tileData);
            }
        }

        public void UpdateDefaultTile()
        {
            FlipTileData tileData = CreateTileData();
            ShellTile tile = ShellTile.ActiveTiles.FirstOrDefault();
            if (tile != null)
            {
                tile.Update(tileData);
            }
        }

        public void UpdateTile()
        {
            var navigationUri = CreateNavigationUri();

            FlipTileData tileData = CreateTileData();

            ShellTile tile = ShellTile.ActiveTiles.FirstOrDefault(t => t.NavigationUri == navigationUri);
            if (tile != null)
            {
                tile.Update(tileData);
            }
        }

        private static Uri CreateNavigationUri()
        {
            var navigationUri = new Uri("/Views/MainPage.xaml", UriKind.Relative);
            return navigationUri;
        }

        private static FlipTileData CreateTileData()
        {
            FlipTileData tileData = new FlipTileData()
            {
                Title = "Where Am I",
                BackTitle = "Where Am I",
                BackContent = string.Empty,
                WideBackContent = string.Empty,
                WideBackgroundImage = new Uri("/Assets/Tiles/FlipCycleTileLarge.png", UriKind.Relative),
                WideBackBackgroundImage = new Uri(@"isostore:\Shared\ShellContent\WhereAmI_Large.jpg", UriKind.Absolute),
                BackgroundImage = new Uri("/Assets/Tiles/FlipCycleTileMedium.png", UriKind.Relative),
                BackBackgroundImage = new Uri(@"isostore:\Shared\ShellContent\WhereAmI_Normal.jpg", UriKind.Absolute),
                SmallBackgroundImage = new Uri("/Assets/Tiles/FlipCycleTileSmall.png", UriKind.Relative)
            };
            return tileData;
        }
    }
}
