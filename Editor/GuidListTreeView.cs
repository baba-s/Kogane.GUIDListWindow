using System;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Kogane.Internal
{
    internal sealed class GuidListTreeView : TreeView
    {
        private enum ColumnType
        {
            GUID,
            ASSET_PATH,
        }

        private GuidListTreeViewItem[] m_list;

        public GuidListTreeView
        (
            TreeViewState     state,
            MultiColumnHeader header
        ) : base( state, header )
        {
            rowHeight                     =  16;
            showAlternatingRowBackgrounds =  true;
            header.sortingChanged         += SortItems;

            Reload();
            header.ResizeToFit();
            header.SetSorting( 1, true );
        }

        protected override TreeViewItem BuildRoot()
        {
            // 要素が存在しない場合、 TreeView は例外を発生する
            // そのため、要素が存在しない場合は表示しないダミーデータを追加する
            m_list = AssetDatabase
                    .GetAllAssetPaths()
                    .OrderBy( x => x )
                    .Select( ( x, index ) => new GuidListTreeViewItem( index, x ) )
                    .DefaultIfEmpty( new GuidListTreeViewItem( 0, string.Empty ) )
                    .ToArray()
                ;

            var root = new TreeViewItem { depth = -1 };

            foreach ( var x in m_list )
            {
                root.AddChild( x );
            }

            return root;
        }

        protected override void RowGUI( RowGUIArgs args )
        {
            var item = ( GuidListTreeViewItem )args.item;

            for ( var i = 0; i < args.GetNumVisibleColumns(); i++ )
            {
                var cellRect   = args.GetCellRect( i );
                var columnType = ( ColumnType )args.GetColumn( i );

                switch ( columnType )
                {
                    case ColumnType.GUID:
                        EditorGUI.LabelField( cellRect, item.Guid );
                        break;

                    case ColumnType.ASSET_PATH:
                        var icon = AssetDatabase.GetCachedIcon( item.AssetPath );

                        var content = new GUIContent
                        {
                            image = icon,
                            text  = item.AssetPath,
                        };

                        EditorGUI.LabelField( cellRect, content );
                        break;

                    default: throw new ArgumentOutOfRangeException();
                }
            }
        }

        /// <summary>
        /// 検索される時に呼び出されます
        /// </summary>
        protected override bool DoesItemMatchSearch( TreeViewItem treeViewItem, string search )
        {
            if ( string.IsNullOrEmpty( search ) ) return true;

            var item = ( GuidListTreeViewItem )treeViewItem;

            return
                item.Guid.Contains( search, StringComparison.OrdinalIgnoreCase ) ||
                item.AssetPath.Contains( search, StringComparison.OrdinalIgnoreCase )
                ;
        }

        /// <summary>
        /// ソートされる時に呼び出されます
        /// </summary>
        private void SortItems( MultiColumnHeader header )
        {
            var sortedColumnIndex = header.sortedColumnIndex;
            var sortedColumnType  = ( ColumnType )sortedColumnIndex;
            var isSortedAscending = header.IsSortedAscending( sortedColumnIndex );

            var sortedList = sortedColumnType switch
            {
                ColumnType.GUID       => m_list.OrderBy( x => x.Guid ),
                ColumnType.ASSET_PATH => m_list.OrderBy( x => x.AssetPath ),
                _                     => throw new ArgumentOutOfRangeException()
            };

            var reversedList = isSortedAscending ? sortedList : sortedList.Reverse();

            rootItem.children = reversedList
                    .Cast<TreeViewItem>()
                    .ToList()
                ;

            BuildRows( rootItem );
        }

        /// <summary>
        /// ダブルクリックされた時に呼び出されます
        /// </summary>
        protected override void DoubleClickedItem( int id )
        {
            var item = m_list.FirstOrDefault( x => x.id == id );

            if ( item == null ) return;

            var asset = AssetDatabase.LoadAssetAtPath<Object>( item.AssetPath );
            EditorGUIUtility.PingObject( asset );
        }
    }
}