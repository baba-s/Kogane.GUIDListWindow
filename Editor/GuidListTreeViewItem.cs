using UnityEditor;
using UnityEditor.IMGUI.Controls;

namespace Kogane.Internal
{
    internal sealed class GuidListTreeViewItem : TreeViewItem
    {
        public string AssetPath { get; }
        public string Guid      { get; }

        public GuidListTreeViewItem( int id, string assetPath ) : base( id )
        {
            AssetPath = assetPath;
            Guid      = AssetDatabase.AssetPathToGUID( AssetPath );
        }
    }
}