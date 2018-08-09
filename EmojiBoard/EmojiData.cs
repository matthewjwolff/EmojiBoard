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

using System.Collections.Generic;

namespace EmojiBoard
{
    public class EmojiData
    {
		public string annotation { get; set; }
		public IList<string> shortcodes { get; set; }
		public IList<string> tags { get; set; }
		public string unicode { get; set; }

		public override bool Equals(object obj)
		{
			return obj is EmojiData && this.unicode.Equals((obj as EmojiData).unicode);
		}

		public override int GetHashCode()
		{
			return unicode.GetHashCode();
		}
		public override string ToString()
		{
			return unicode + " " + annotation;
		}
	}
}