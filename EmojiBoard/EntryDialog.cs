/* EmojiBoard: Helps input unicode emoji characters
 * Copyright 2018 Matthew Wolff. 
 * https://github.com/matthewjwolff/EmojiBoard
 *
 * EmojiBoard is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published
 * by the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * EmojiBoard is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using Gtk;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using LibUIOHookNet;

namespace EmojiBoard
{
    public partial class EntryDialog : Window
    {
		private readonly TreeModelFilter searchFilter;
		private readonly StatusIcon StatusIcon;

		public EntryDialog(IList<EmojiData> data) :
				base(WindowType.Toplevel)
		{
			this.Build();
			this.StatusIcon = new StatusIcon("thinking.png");
			this.StatusIcon.Tooltip = "EmojiBoard is running";
			this.StatusIcon.PopupMenu += StatusIcon_PopupMenu;
			ListStore store = new ListStore(typeof(EmojiData));
			this.EmojiView.AppendColumn("Emoji", new CellRendererText(), RenderEmojiData);
			foreach (var emoji in data)
			{
				store.AppendValues(emoji);
			}
			this.searchFilter = new TreeModelFilter(store, null);
			this.searchFilter.VisibleFunc = new TreeModelFilterVisibleFunc(HandleTreeModelFilterVisibleFunc);
			this.EmojiView.Model = searchFilter;

			UIOHook.OnKeyType += UIOHook_OnKeyType;
		}

		void StatusIcon_PopupMenu(object o, PopupMenuArgs args)
		{
			Menu popupMenu = new Menu();
            ImageMenuItem menuItemQuit = new ImageMenuItem("Stop EmojiBoard");
			Image appimg = new Image(Stock.Quit, IconSize.Menu);
            menuItemQuit.Image = appimg;
            popupMenu.Add(menuItemQuit);
            // Quit the application when quit has been clicked.
            menuItemQuit.Activated += delegate { Application.Quit(); };
            popupMenu.ShowAll();
            popupMenu.Popup();
		}


		// TODO: define constants in library binding
        public static readonly ushort MASK_SHIFT_L = 1 << 0;
        public static readonly ushort MASK_CTRL_L = 1 << 1;

        private void UIOHook_OnKeyType(object sender, KeyTypeEventArgs e)
        {
            if (e.keychar == ':' && e.mask == (MASK_CTRL_L | MASK_SHIFT_L))
            {
				Gtk.Application.Invoke(delegate
				{
					if (!this.Visible)
					{
						this.Show();
						this.TextEntry.GrabFocus();
					}
				});

            }

        }

		bool HandleTreeModelFilterVisibleFunc(TreeModel model, TreeIter iter)
		{
			if(this.TextEntry.Text.Equals(""))
			{
				return true;
			}
			EmojiData value = model.GetValue(iter, 0) as EmojiData;
			return value.annotation.IndexOf(this.TextEntry.Text) > -1;
		}


		private void RenderEmojiData(TreeViewColumn tree_column, CellRenderer cell, TreeModel tree_model, TreeIter iter)
		{
			EmojiData value = tree_model.GetValue(iter, 0) as EmojiData;
			(cell as CellRendererText).Text = value.unicode + " " + value.annotation;
		}      

		public static void Main(string[] args)
		{
			// load emoji into memory
            string contents = File.ReadAllText("emoji.json");
			IList<EmojiData> emojis = JsonConvert.DeserializeObject<IList<EmojiData>>(contents).OrderBy(e=>e.annotation).ToList();

			// if you put the following line between Application.Init and Application.Run, you'll segfault when you call Window.Show()
			UIOHook.StartHook();
			Application.Init();
			mainDialog = new EntryDialog(emojis);
			mainDialog.Hide();
			Application.Run();
			// The application has quit, shut down the hook thread
			UIOHook.StopHook();

		}

		private static EntryDialog mainDialog;

		protected void OnDeleteEvent(object o, DeleteEventArgs args)
		{
			Application.Quit();
		}

		protected void OnTextEntryActivated(object sender, EventArgs e)
		{
			Clipboard.Get(Gdk.Atom.Intern("CLIPBOARD", true)).Text = this.TextEntry.Text;
			mainDialog.Hide();
		}

		protected void OnTextEntryChanged(object sender, EventArgs e)
		{
			this.searchFilter.Refilter();
		}
	}
}
