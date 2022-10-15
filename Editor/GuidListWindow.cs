using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Kogane.Internal
{
    internal sealed class GuidListWindow : EditorWindow
    {
        private const string SEARCH_STRING_STATE_KEY = "GuidListWindow_SearchString";

        private static readonly GUILayoutOption   WIDTH_OPTION  = GUILayout.Width( 16 );
        private static readonly GUILayoutOption   HEIGHT_OPTION = GUILayout.Height( EditorGUIUtility.singleLineHeight );
        private static readonly GUILayoutOption[] OPTIONS       = { WIDTH_OPTION, HEIGHT_OPTION };

        private static GUIContent m_refreshIcon;

        private SearchField      m_searchField;
        private GuidListHeader   m_header;
        private GuidListTreeView m_treeView;

        private void OnEnable()
        {
            var state = new TreeViewState();

            m_header = new( null );

            m_treeView = new( state, m_header )
            {
                searchString = SessionState.GetString( SEARCH_STRING_STATE_KEY, string.Empty )
            };

            m_searchField                         =  new();
            m_searchField.downOrUpArrowKeyPressed += m_treeView.SetFocusAndEnsureSelectedItem;
        }

        private void OnGUI()
        {
            using ( new EditorGUILayout.HorizontalScope() )
            {
                DrawSearchField();
                DrawRefreshButton();
            }

            DrawTreeView();
        }

        private void ReloadTreeView()
        {
            m_treeView.Reload();

            var index = m_header.sortedColumnIndex;
            m_header.SetSorting( index, !m_header.IsSortedAscending( index ) );
            m_header.SetSorting( index, !m_header.IsSortedAscending( index ) );
        }

        private void DrawSearchField()
        {
            using var scope = new EditorGUI.ChangeCheckScope();

            var searchString = m_searchField.OnToolbarGUI( m_treeView.searchString );

            if ( !scope.changed ) return;

            SessionState.SetString( SEARCH_STRING_STATE_KEY, searchString );
            m_treeView.searchString = searchString;
        }

        private void DrawRefreshButton()
        {
            m_refreshIcon ??= EditorGUIUtility.IconContent( "d_Refresh" );

            using var scope = new EditorGUILayout.VerticalScope( OPTIONS );

            GUILayout.FlexibleSpace();

            if ( GUILayout.Button( m_refreshIcon, EditorStyles.iconButton ) )
            {
                ReloadTreeView();
            }

            GUILayout.FlexibleSpace();
        }

        private void DrawTreeView()
        {
            var singleLineHeight = EditorGUIUtility.singleLineHeight;

            var rect = new Rect
            {
                x      = 0,
                y      = singleLineHeight + 1,
                width  = position.width,
                height = position.height - singleLineHeight - 1
            };

            m_treeView.OnGUI( rect );
        }

        [MenuItem( "Window/Kogane/GUID List", false, 1273143669 )]
        private static void Open()
        {
            GetWindow<GuidListWindow>( "GUID List" );
        }
    }
}