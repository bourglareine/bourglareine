//------------------------------------------------------------------------------
// <auto-generated>
//     Ce code a été généré par un outil.
//     Version du runtime :4.0.30319.34209
//
//     Les modifications apportées à ce fichier peuvent provoquer un comportement incorrect et seront perdues si
//     le code est régénéré.
// </auto-generated>
//------------------------------------------------------------------------------
using System;
using UnityEngine;
namespace AssemblyCSharpfirstpass
{
		public class ButtonCreator
		{
				public ButtonCreator ()
				{
				}


				public void CreateButton (int buttonWidth, int buttonHeight, int screenHeight, int screenWidth)
				{
						Rect buttonRect = new Rect (
				Screen.width / 2 - (buttonWidth / 2),
				(2 * Screen.height / 3) - (buttonHeight / 2),
				buttonWidth,
				buttonHeight
						);
				}
		}
}

