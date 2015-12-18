using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ReadingList
{
    public partial class Search : Form
    {
        const string grKey = "HQ2Bd6VkF1VOJYsYmfyiRA";
        const string grSearchString = "http://goodreads.com/search.xml?key={0}&q={1}&page={2}";
        private object timerLock = new object();
        private const int delayInterval = 1000;
        System.Timers.Timer delayTimer = new System.Timers.Timer(delayInterval);
        private int queryQueued = 0;
        private string queryString = string.Empty;
        private GoodreadsWork selectedWork;

        public bool HasResult { get { return this.selectedWork != null; } }

        public string ResultTitle { get { return this.selectedWork.Book.Title; } }

        public string ResultAuthor { get { return this.selectedWork.Book.Author.Name; } }

        public float ResultRating { get { return this.selectedWork.AverageRating; } }

        private void DelayTimerCallback(object sender, EventArgs e)
        {
            if (Interlocked.Exchange(ref queryQueued, 1) == 0)
            {
                var requestString = GetRequestString(queryString);
                var results = SearchGoodreads(requestString).ToList();

                this.okButton.Invoke(new Action(() =>
                {
                    this.okButton.Enabled = false;
                }));

                bool selected = false;
                this.selectionBox.Invoke(new Action(() => {
                    this.selectionBox.ClearSelected();
                    this.selectionBox.Items.Clear();
                    foreach (var result in results)
                    {
                        this.selectionBox.Items.Add(result);

                        if (!selected && result.Equals(this.selectedWork))
                        {
                            this.selectionBox.SelectedItem = result;
                            selected = true;
                        }
                    }

                    if (selected == false)
                    {
                        this.selectedWork = null;
                    }
                }));

                if (selected)
                {
                    this.okButton.Invoke(new Action(() =>
                    {
                        this.okButton.Enabled = true;
                    }));
                }

                Interlocked.Exchange(ref queryQueued, 0);
            }
        }

        private string GetRequestString(string queryString)
        {
            return string.Format(CultureInfo.InvariantCulture, grSearchString, grKey, queryString, string.Empty); 
        }

        private IEnumerable<GoodreadsWork> SearchGoodreads(string requestString)
        {
            var uri = new Uri(requestString);
            var request = (HttpWebRequest)WebRequest.Create(uri.AbsoluteUri);
            var response = (HttpWebResponse)request.GetResponse();

            if (response.StatusCode != HttpStatusCode.OK)
            {
                return new List<GoodreadsWork>();
            }

            var encoding = Encoding.ASCII;
            string responseText = string.Empty;
            using (var reader = new System.IO.StreamReader(response.GetResponseStream(), encoding))
            {
                responseText = reader.ReadToEnd();
            }

            return GoodreadsWork.ParseMany(responseText);
        }
                
        public Search()
        {
            delayTimer.AutoReset = false;
            delayTimer.Elapsed += DelayTimerCallback;
            InitializeComponent();
            this.TopMost = true;
        }

        private void queryBox_TextChanged(object sender, EventArgs e)
        {
            queryString = (sender as TextBox)?.Text ?? string.Empty;

            delayTimer.Stop();
            delayTimer.Start();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void selectionBox_Select(object sender, EventArgs e)
        {
            var box = sender as ListBox;

            this.selectedWork = box.SelectedItem as GoodreadsWork;
            this.okButton.Invoke(new Action(() =>
            {
                this.okButton.Enabled = true;
            }));
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            this.Hide();
        }
    }
}
