using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SkillKeeper
{
    public class HtmlGenerator
    {
        private static readonly string StyleDefinition = "<style type=\"text/css\">" + Environment.NewLine +
            ".tg  {border-collapse:collapse;border-spacing:0;border-color:#ccc;}" + Environment.NewLine +
            ".tg td{font-family:Arial, sans-serif;font-size:14px;padding:10px 5px;border-style:solid;border-width:1px;overflow:hidden;word-break:normal;border-color:#ccc;color:#333;background-color:#fff;}" + Environment.NewLine +
            ".tg th{font-family:Arial, sans-serif;font-size:14px;font-weight:normal;padding:10px 5px;border-style:solid;border-width:1px;overflow:hidden;word-break:normal;border-color:#ccc;color:#333;background-color:#f0f0f0;}" + Environment.NewLine +
            ".tg .style1{vertical-align:top}" + Environment.NewLine +
            "</style>" + Environment.NewLine;

        private static readonly string TableStart = "<table class=\"tg\" style=\"undefined;table-layout: fixed;\">" + Environment.NewLine +
            "<colgroup>" + Environment.NewLine +
            "<col style=\"width: 55px\">" + Environment.NewLine +
            "<col style=\"width: 80px\">" + Environment.NewLine +
            "<col style=\"width: 120px\" >" + Environment.NewLine +
            "</colgroup>" + Environment.NewLine;

        private static readonly string TableEnd = "</table>" + Environment.NewLine;

        private static readonly string TableRowHeader = "<tr>" + Environment.NewLine +
            "<th class=\"style1\">Ranking</th>" + Environment.NewLine +
            "<th class=\"style1\">Player</th>" + Environment.NewLine +
            "<th class=\"style1\">Character</th>" + Environment.NewLine +
            "</tr>" + Environment.NewLine;

        private static readonly string TableRowData = "<tr>" + Environment.NewLine +
            "<td class=\"style1\">{0}</td>" + Environment.NewLine +
            "<td class=\"style1\">{1}</td>" + Environment.NewLine +
            "<td class=\"style1\">{2}</td>" + Environment.NewLine +
            "</tr>" + Environment.NewLine;

        private static readonly string ImgSrc = "<img src=\"{0}\" width=\"24\" height=\"24\">";

        private Dictionary<string, List<string>> playerCharacterDictionary;

        /// <summary>
        /// Creates a new instance of HtmlMaker, based on the leaderboard grid.
        /// Parses leaderboard grid into a dictionary of player:characters.
        /// </summary>
        /// <param name="leaderBoardGrid">Leaderboard grid object.</param>
        public HtmlGenerator(DataGridView leaderBoardGrid)
        {
            this.playerCharacterDictionary = new Dictionary<string, List<string>>();
            foreach (DataGridViewRow row in leaderBoardGrid.Rows)
            {
                var cells = row.Cells.Cast<DataGridViewCell>();
                var nameCell = cells.First(c => c.OwningColumn.HeaderText == "Name");
                var charCell = cells.First(c => c.OwningColumn.HeaderText == "Characters");
                this.playerCharacterDictionary.Add(nameCell.Value.ToString(), charCell.Value.ToString().Split(';').Select(c => c.Trim().ToLowerInvariant()).ToList());
            }
        }

        /// <summary>
        /// Returns the leaderboard grid as an HTML table.
        /// </summary>
        /// <param name="numTopPlayers">Number of players to include (counted from the top).</param>
        /// <returns>Leaderboard grid as an HTML table.</returns>
        public string GetGridAsHtml(int numTopPlayers)
        {
            StringBuilder html = new StringBuilder();
            html.Append(HtmlGenerator.StyleDefinition);
            html.Append(HtmlGenerator.TableStart);
            html.Append(HtmlGenerator.TableRowHeader);
            html.Append(GetTableRowData(numTopPlayers));
            html.Append(HtmlGenerator.TableEnd);

            return html.ToString();
        }

        /// <summary>
        /// Iterates through player:character dictionary and generates the table data.
        /// </summary>
        /// <param name="numTopPlayers">Number of players to include (counted from the top).</param>
        /// <returns>Player-portion (table data) of the leaderboard table.</returns>
        private string GetTableRowData(int numTopPlayers)
        {
            if (numTopPlayers > this.playerCharacterDictionary.Count)
            {
                numTopPlayers = this.playerCharacterDictionary.Count;
            }

            StringBuilder html = new StringBuilder();
            for (int rank = 1; rank <= numTopPlayers; rank++)
            {
                var player = this.playerCharacterDictionary.ElementAt(rank - 1);
                html.Append(string.Format(HtmlGenerator.TableRowData, rank, player.Key, GetImgUrls(player.Value)));
            }

            return html.ToString();
        }

        /// <summary>
        /// Reads the ini file and uses the "iconurls" section to generate image tags for each player's character.
        /// </summary>
        /// <param name="characters">Player's character list.</param>
        /// <returns>HTML image tags for all of the player's characters.</returns>
        private string GetImgUrls(List<string> characters)
        {
            StringBuilder imgUrls = new StringBuilder();
            IniFile iniFile = IniFile.GetInstance();
            foreach (var character in characters)
            {
                try
                {
                    string url = iniFile.Data[IniFile.IconUrlsSectionName][character];
                    if (!string.IsNullOrWhiteSpace(url))
                    {
                        imgUrls.Append(string.Format(HtmlGenerator.ImgSrc, url));
                    }
                }
                catch (NullReferenceException)
                {
                    // Skip if character was not found in ini file.
                }
            }
            return imgUrls.ToString();
        }
    }
}
