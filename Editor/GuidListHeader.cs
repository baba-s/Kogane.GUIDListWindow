using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Kogane.Internal
{
    internal sealed class GuidListHeader : MultiColumnHeader
    {
        private static GUIContent m_guidHeaderContent;
        private static GUIContent m_assetPathHeaderContent;

        public GuidListHeader( MultiColumnHeaderState state ) : base( state )
        {
            const int guidWidth = 288;

            var columns = new MultiColumnHeaderState.Column[]
            {
                new()
                {
                    width               = guidWidth,
                    minWidth            = guidWidth,
                    maxWidth            = guidWidth,
                    headerContent       = m_guidHeaderContent ??= new( "GUID" ),
                    headerTextAlignment = TextAlignment.Center,
                },
                new()
                {
                    headerContent       = m_assetPathHeaderContent ??= new( "Asset Path" ),
                    headerTextAlignment = TextAlignment.Center,
                },
            };

            this.state = new( columns );
        }
    }
}