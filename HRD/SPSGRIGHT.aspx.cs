using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Oracle.ManagedDataAccess.Client;

namespace HRD
{
    public partial class SPSGRIGHT : System.Web.UI.Page
    {
        // Data structures for Tabs and Notices
        public class TabItem
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public int? TabOrder { get; set; }
            public List<UploadItem> Items { get; set; }

            public TabItem()
            {
                Items = new List<UploadItem>();
            }
        }

        public class UploadItem
        {
            public string FileSlNo { get; set; }
            public string FileDetails { get; set; }
            public string FileAttach { get; set; }
            public string FileRemarks { get; set; }
            public string FileStructId { get; set; }
            public int FileStatus { get; set; }
            public DateTime? FileUploadDate { get; set; }
            public DateTime? TillShowDate { get; set; }
            public string UploadTabId { get; set; }
            public bool IsNew { get; set; }
        }

        /// <summary>
        /// Configurable table names. Can be set in Web.config &lt;appSettings&gt;
        /// e.g.: &lt;add key="TabTableName" value="your_table_name" /&gt;
        /// or changed directly here in code.
        /// </summary>
        public string TabTableName
        {
            get
            {
                string configured = ConfigurationManager.AppSettings["TabTableName"];
                return !string.IsNullOrWhiteSpace(configured) ? configured.Trim() : "spsgtabname";
            }
        }

        public string UploadTableName
        {
            get
            {
                string configured = ConfigurationManager.AppSettings["UploadTableName"];
                return !string.IsNullOrWhiteSpace(configured) ? configured.Trim() : "spsgupload";
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadPortalData();
            }
        }

        /// <summary>
        /// Retrieves the Oracle connection string from Web.config.
        /// Checks 'OracleConn', 'HRDConnection', or 'DefaultConnection'.
        /// </summary>
        private string GetConnectionString()
        {
            if (ConfigurationManager.ConnectionStrings["OracleConn"] != null &&
                !string.IsNullOrWhiteSpace(ConfigurationManager.ConnectionStrings["OracleConn"].ConnectionString))
            {
                return ConfigurationManager.ConnectionStrings["OracleConn"].ConnectionString;
            }

            if (ConfigurationManager.ConnectionStrings["HRDConnection"] != null &&
                !string.IsNullOrWhiteSpace(ConfigurationManager.ConnectionStrings["HRDConnection"].ConnectionString))
            {
                return ConfigurationManager.ConnectionStrings["HRDConnection"].ConnectionString;
            }

            if (ConfigurationManager.ConnectionStrings["DefaultConnection"] != null &&
                !string.IsNullOrWhiteSpace(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                return ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            }

            if (!string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["OracleConnectionString"]))
            {
                return ConfigurationManager.AppSettings["OracleConnectionString"];
            }

            return "User Id=system;Password=root;Data Source=//127.0.0.1:1521/xe;Min Pool Size=2;Max Pool Size=100;Connection Lifetime=120;Statement Cache Size=20;Validate Connection=false;";
        }

        /// <summary>
        /// Loads tab definitions from table 'spsgtabname' and notices from table 'spsgupload'.
        /// Queries live data directly from the Oracle database.
        /// </summary>
        private void LoadPortalData()
        {
            List<TabItem> tabs = new List<TabItem>();
            string connStr = GetConnectionString();

            try
            {
                using (OracleConnection conn = new OracleConnection(connStr))
                {
                    conn.Open();

                    // 1. Fetch Dynamic Tabs from configured tab table ordered by taborder
                    string tabSql = string.Format("SELECT id, name, taborder FROM {0} ORDER BY NVL(taborder, id) ASC, id ASC", TabTableName);
                    using (OracleCommand cmd = new OracleCommand(tabSql, conn))
                    using (OracleDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            tabs.Add(new TabItem
                            {
                                Id = reader["id"] != DBNull.Value ? reader["id"].ToString().Trim() : "",
                                Name = reader["name"] != DBNull.Value ? reader["name"].ToString().Trim() : "General",
                                TabOrder = reader["taborder"] != DBNull.Value ? Convert.ToInt32(reader["taborder"]) : (int?)null
                            });
                        }
                    }

                    // 2. Fetch Active Notices from configured upload table
                    // Condition: filestatus = 1 and (tillshowdate >= TRUNC(SYSDATE) or tillshowdate is null)
                    string uploadSql = string.Format(@"SELECT fileslno, filedetails, fileattach, fileremarks, filestructid, 
                                                filestatus, fileuploaddate, tillshowdate, uploadtabid 
                                         FROM {0} 
                                         WHERE filestatus = 1 
                                           AND (tillshowdate >= TRUNC(SYSDATE) OR tillshowdate IS NULL) 
                                         ORDER BY fileuploaddate DESC, fileslno DESC", UploadTableName);

                    Dictionary<string, List<UploadItem>> uploadMap = new Dictionary<string, List<UploadItem>>(StringComparer.OrdinalIgnoreCase);

                    using (OracleCommand cmd = new OracleCommand(uploadSql, conn))
                    using (OracleDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            UploadItem item = new UploadItem();
                            item.FileSlNo = reader["fileslno"] != DBNull.Value ? reader["fileslno"].ToString() : "";
                            item.FileDetails = reader["filedetails"] != DBNull.Value ? reader["filedetails"].ToString() : "";
                            item.FileAttach = reader["fileattach"] != DBNull.Value ? reader["fileattach"].ToString() : "";
                            item.FileRemarks = reader["fileremarks"] != DBNull.Value ? reader["fileremarks"].ToString() : "";
                            item.FileStructId = reader["filestructid"] != DBNull.Value ? reader["filestructid"].ToString() : "";
                            item.FileStatus = reader["filestatus"] != DBNull.Value ? Convert.ToInt32(reader["filestatus"]) : 0;
                            item.UploadTabId = reader["uploadtabid"] != DBNull.Value ? reader["uploadtabid"].ToString().Trim() : "";

                            if (reader["fileuploaddate"] != DBNull.Value)
                            {
                                item.FileUploadDate = Convert.ToDateTime(reader["fileuploaddate"]);
                                // Consider notice "NEW" if uploaded in last 14 days
                                if ((DateTime.Today - item.FileUploadDate.Value.Date).TotalDays <= 14 &&
                                    (DateTime.Today - item.FileUploadDate.Value.Date).TotalDays >= 0)
                                {
                                    item.IsNew = true;
                                }
                            }

                            if (reader["tillshowdate"] != DBNull.Value)
                            {
                                item.TillShowDate = Convert.ToDateTime(reader["tillshowdate"]);
                            }

                            if (!uploadMap.ContainsKey(item.UploadTabId))
                            {
                                uploadMap[item.UploadTabId] = new List<UploadItem>();
                            }
                            uploadMap[item.UploadTabId].Add(item);
                        }
                    }

                    // Assign items to respective tabs
                    foreach (var tab in tabs)
                    {
                        if (uploadMap.ContainsKey(tab.Id))
                        {
                            tab.Items = uploadMap[tab.Id];
                        }
                    }
                }

                litStatusBanner.Text = string.Empty;
            }
            catch (Exception ex)
            {
                litStatusBanner.Text = string.Format(
                    @"<div class=""status-banner error-banner"">
                        <span class=""status-badge error-badge"">Database Error</span>
                        <span>Failed to fetch records from Oracle database: {0}</span>
                      </div>",
                    HttpUtility.HtmlEncode(ex.Message)
                );
            }

            // Render Tabs and Content
            RenderTabsAndPanels(tabs);
        }

        /// <summary>
        /// Generates the HTML for tab buttons and corresponding panels.
        /// </summary>
        private void RenderTabsAndPanels(List<TabItem> tabs)
        {
            if (tabs == null || tabs.Count == 0)
            {
                litTabsNav.Text = string.Format("<div class=\"no-tabs\">No categories found in {0}.</div>", HttpUtility.HtmlEncode(TabTableName));
                litTabsContent.Text = string.Format("<div class=\"empty-content\"><div class=\"empty-state\"><div class=\"empty-icon\">&#128196;</div><div class=\"empty-title\">No tab categories available</div><div class=\"empty-desc\">Please verify that table <strong>{0}</strong> contains records.</div></div></div>", HttpUtility.HtmlEncode(TabTableName));
                return;
            }

            StringBuilder navHtml = new StringBuilder();
            StringBuilder contentHtml = new StringBuilder();

            for (int i = 0; i < tabs.Count; i++)
            {
                TabItem tab = tabs[i];
                string activeClass = (i == 0) ? " active" : "";
                string displayStyle = (i == 0) ? "display: block;" : "display: none;";
                string tabDomId = "tab-pane-" + HttpUtility.HtmlAttributeEncode(tab.Id);

                // Tab Button
                navHtml.AppendFormat(
                    @"<button type=""button"" class=""tab-nav-btn{0}"" onclick=""switchTab('{1}', this)"" data-tab-id=""{1}"">
                        <svg class=""tab-icon"" width=""15"" height=""15"" viewBox=""0 0 24 24"" fill=""none"" stroke=""currentColor"" stroke-width=""2"" stroke-linecap=""round"" stroke-linejoin=""round""><path d=""M14 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V8z""></path><polyline points=""14 2 14 8 20 8""></polyline><line x1=""16"" y1=""13"" x2=""8"" y2=""13""></line><line x1=""16"" y1=""17"" x2=""8"" y2=""17""></line></svg>
                        <span class=""tab-name"">{2}</span>
                        <span class=""tab-badge"" title=""{3} Notices"">{3}</span>
                      </button>",
                    activeClass,
                    tabDomId,
                    HttpUtility.HtmlEncode(tab.Name),
                    tab.Items.Count
                );

                // Tab Content Panel
                contentHtml.AppendFormat(
                    @"<div id=""{0}"" class=""tab-panel{1}"" style=""{2}"" data-item-count=""{3}"">",
                    tabDomId,
                    activeClass,
                    displayStyle,
                    tab.Items.Count
                );

                if (tab.Items.Count == 0)
                {
                    contentHtml.AppendFormat(
                        @"<div class=""empty-state"">
                            <div class=""empty-icon"">&#128196;</div>
                            <div class=""empty-title"">No active notices</div>
                            <div class=""empty-desc"">Currently there are no active documents or notices under <strong>{0}</strong>.</div>
                          </div>",
                        HttpUtility.HtmlEncode(tab.Name)
                    );
                }
                else
                {
                    // Ticker / Scrollable container
                    contentHtml.AppendFormat(
                        @"<div class=""ticker-viewport"" id=""viewport-{0}"" onmouseenter=""pauseTicker('{0}')"" onmouseleave=""resumeTicker('{0}')"">
                            <div class=""ticker-content"" id=""content-{0}"">",
                        tabDomId
                    );

                    for (int j = 0; j < tab.Items.Count; j++)
                    {
                        UploadItem item = tab.Items[j];
                        string dateStr = item.FileUploadDate.HasValue
                            ? item.FileUploadDate.Value.ToString("dd-MM-yyyy")
                            : "Recent";

                        // Check if fileattach is a complete URL or a local uploaded PDF filename
                        string rawAttach = (item.FileAttach ?? "").Trim();
                        string pdfUrl;

                        if (string.IsNullOrEmpty(rawAttach))
                        {
                            pdfUrl = "javascript:void(0);";
                        }
                        else if (rawAttach.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                                 rawAttach.StartsWith("https://", StringComparison.OrdinalIgnoreCase) ||
                                 rawAttach.StartsWith("ftp://", StringComparison.OrdinalIgnoreCase) ||
                                 rawAttach.StartsWith("//"))
                        {
                            // Complete URL specified in database (points directly to that URL)
                            pdfUrl = rawAttach;
                        }
                        else if (rawAttach.StartsWith("www.", StringComparison.OrdinalIgnoreCase))
                        {
                            // Complete URL starting with www
                            pdfUrl = "http://" + rawAttach;
                        }
                        else if (rawAttach.StartsWith("/") || rawAttach.StartsWith("~/"))
                        {
                            pdfUrl = ResolveUrl(rawAttach);
                        }
                        else
                        {
                            // Local PDF filename saved in SPSGUPLOAD/UPLOADFILES/ folder
                            pdfUrl = "SPSGUPLOAD/UPLOADFILES/" + rawAttach;
                        }

                        string remarksHtml = !string.IsNullOrWhiteSpace(item.FileRemarks)
                            ? string.Format(@"<div class=""item-remarks"">&#9432; {0}</div>", HttpUtility.HtmlEncode(item.FileRemarks))
                            : "";

                        contentHtml.AppendFormat(
                            @"<div class=""notice-card"" data-index=""{0}"" data-search=""{1}"">
                                <a href=""{2}"" target=""_blank"" class=""notice-link"" title=""Click to view/download PDF"">
                                    <div class=""notice-row"">
                                        <div class=""notice-date"">
                                            <span class=""date-icon"">&#128197;</span>
                                            <span class=""date-text"">{3}</span>
                                        </div>
                                        <div class=""notice-title-box"">
                                            <span class=""notice-title"">{4}</span>
                                            {5}
                                        </div>
                                    </div>
                                </a>
                              </div>",
                            j,
                            HttpUtility.HtmlAttributeEncode((item.FileDetails + " " + item.FileRemarks + " " + dateStr).ToLower()),
                            HttpUtility.HtmlAttributeEncode(pdfUrl),
                            HttpUtility.HtmlEncode(dateStr),
                            HttpUtility.HtmlEncode(item.FileDetails),
                            remarksHtml
                        );
                    }

                    contentHtml.Append("</div></div>"); // Close ticker-content and ticker-viewport
                }

                contentHtml.Append("</div>"); // Close tab-panel
            }

            litTabsNav.Text = navHtml.ToString();
            litTabsContent.Text = contentHtml.ToString();
        }
    }
}