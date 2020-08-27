using System;
using System.Net;
using System.Threading;
using System.Web.Security;

using CMS.Base;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_System_Debug_System_DebugLoad : CMSDebugPage
{
    #region "Variables"

    private class LoadSettings
    {
        public bool Cancel = true;
        public bool Run;
        public int SuccessRequests;
        public int Errors;
        public int CurrentThreads;
        public string UserName = "";
        public string Duration = "";
        public string Iterations = "1000";
        public string Interval = "";
        public string Threads = "10";
        public string UserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 6.1; WOW64; Trident/5.0; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; Media Center PC 6.0; InfoPath.2; .NET4.0C; .NET4.0E; MS-RTC LM 8)";
        public string URLs = "~/Home.aspx";
        public bool SplitURLs;

        public string LastError;
    }

    #endregion


    #region "Request loader"

    /// <summary>
    /// Request loader class.
    /// </summary>
    protected class RequestLoader
    {
        public string[] URLs = null;
        public DateTime RunUntil = DateTimeHelper.ZERO_TIME;
        public int NumberOfIterations = -1;
        public int WaitInterval = 0;
        public string UserAgent = null;
        public string UserName = null;


        /// <summary>
        /// Returns true if the loader is canceled (exceeds the execution time, exceeds an allowed number of iterations or is forcibly canceled).
        /// </summary>
        protected bool IsCanceled()
        {
            return Settings.Cancel || ((RunUntil != DateTimeHelper.ZERO_TIME) && (DateTime.Now > RunUntil)) || ((NumberOfIterations != -1) && (NumberOfIterations == 0));
        }


        /// <summary>
        /// Runs the load to the URLs.
        /// </summary>
        public void Run()
        {
            var s = Settings;

            s.CurrentThreads++;

            // Prepare the client
            WebClient client = new WebClient();

            // Authenticate specified user
            if (!string.IsNullOrEmpty(UserName))
            {
                client.Headers.Add("Cookie", ".ASPXFORMSAUTH=" + FormsAuthentication.GetAuthCookie(UserName, false).Value);
            }

            // Add user agent header
            if (!string.IsNullOrEmpty(UserAgent))
            {
                client.Headers.Add("user-agent", UserAgent);
            }

            while (!IsCanceled())
            {
                // Run the list of URLs
                foreach (string url in URLs)
                {
                    if (!string.IsNullOrEmpty(url))
                    {
                        if (IsCanceled())
                        {
                            break;
                        }

                        // Wait if some interval specified
                        if (WaitInterval > 0)
                        {
                            Thread.Sleep(WaitInterval);
                        }

                        try
                        {
                            // Get the page
                            client.DownloadData(url);

                            s.SuccessRequests++;
                        }
                        catch (Exception ex)
                        {
                            s.LastError = ex.Message;
                            s.Errors++;
                        }
                    }
                }

                // Decrease number of iterations
                if (NumberOfIterations > 0)
                {
                    NumberOfIterations--;
                }
            }

            // Dispose the client
            client.Dispose();

            s.CurrentThreads--;
        }
    }

    #endregion


    private static readonly LoadSettings Settings = new LoadSettings();


    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        EnsureScriptManager();
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        if (!RequestHelper.IsPostBack())
        {
            if ((Settings.CurrentThreads > 0) || (Settings.SuccessRequests > 0) || (Settings.Errors > 0))
            {
                txtDuration.Text = Settings.Duration;
                txtInterval.Text = Settings.Interval;
                txtIterations.Text = Settings.Iterations;
                txtThreads.Text = Settings.Threads;
                txtURLs.Text = Settings.URLs;
                txtUserAgent.Text = Settings.UserAgent;
                userElem.Value = Settings.UserName;
                chkSplitUrls.Checked = Settings.SplitURLs;
            }
        }
        else
        {
            if (Settings.Run && (Settings.CurrentThreads == 0))
            {
                // Enable the form when the load finished
                Settings.Run = false;
                SetEnabled(true);
                pnlBody.Update();
            }
        }
    }


    protected void Page_PreRender(object sender, EventArgs e)
    {
        lblInfo.Text = String.Format(GetString("DebugLoad.Info"), Settings.CurrentThreads, Settings.SuccessRequests, Settings.Errors);
        if (!String.IsNullOrEmpty(Settings.LastError))
        {
            ShowError(Settings.LastError);
        }

        btnStart.Text = GetString("DebugLoad.Generate");
        btnStop.Text = GetString("DebugLoad.Stop");
        btnReset.Text = GetString("DebugLoad.Reset");

        if (Settings.CurrentThreads > 0)
        {
            SetEnabled(false);
        }
        btnStop.Enabled = (Settings.CurrentThreads > 0);
    }


    protected void btnReset_Click(object sender, EventArgs e)
    {
        Reset();
    }


    private static void Reset()
    {
        var s = Settings;

        s.SuccessRequests = 0;
        s.Errors = 0;
        s.LastError = "";
    }


    protected void btnStop_Click(object sender, EventArgs e)
    {
        Stop();

        SetEnabled(true);
        btnStop.Enabled = false;
    }


    private static void Stop()
    {
        var s = Settings;

        s.Run = false;
        s.Cancel = true;

        while (s.CurrentThreads > 0)
        {
            Thread.Sleep(100);
        }

        s.CurrentThreads = 0;
        s.LastError = "";
    }


    protected void btnStart_Click(object sender, EventArgs e)
    {
        ResetSettings();

        LoadSettingsFromUI();

        if (RunThreads())
        {
            SetEnabled(false);

            btnStop.Enabled = true;
            btnReset.Enabled = true;
        }
    }


    private bool RunThreads()
    {
        var s = Settings;

        if (!String.IsNullOrEmpty(s.URLs))
        {
            int newThreads = ValidationHelper.GetInteger(s.Threads, 0);
            if (newThreads > 0)
            {
                // Prepare the parameters
                string[] urls = s.URLs.Split(new[]
                {
                    '\r',
                    '\n'
                }, StringSplitOptions.RemoveEmptyEntries);

                for (int i = 0; i < urls.Length; i++)
                {
                    urls[i] = URLHelper.GetAbsoluteUrl(urls[i]);
                }

                int duration = ValidationHelper.GetInteger(s.Duration, 0);
                int interval = ValidationHelper.GetInteger(s.Interval, 0);
                int iterations = ValidationHelper.GetInteger(s.Iterations, 0);
                bool splitUrls = ValidationHelper.GetBoolean(s.SplitURLs, false);

                DateTime runUntil = DateTime.Now.AddSeconds(duration);

                // Divide URLs between threads
                string[][] partUrls = null;

                if (splitUrls)
                {
                    // Do not run more threads than URLs
                    newThreads = Math.Min(urls.Length, newThreads);

                    partUrls = new string[newThreads][];

                    int size = (int)Math.Ceiling((double)urls.Length / newThreads);

                    for (int i = 0; i < newThreads; i++)
                    {
                        size = Math.Min(size, urls.Length - i * size);
                        partUrls[i] = new string[size];

                        for (int j = 0; j < size; j++)
                        {
                            partUrls[i][j] = urls[i * size + j];
                        }
                    }
                }

                // Run specified number of threads
                for (int i = 0; i < newThreads; i++)
                {
                    // Prepare the loader object
                    RequestLoader loader = new RequestLoader();

                    loader.URLs = (splitUrls ? partUrls[i] : urls);
                    loader.WaitInterval = interval;
                    loader.UserAgent = s.UserAgent;
                    loader.UserName = ValidationHelper.GetString(userElem.Value, "").Trim();

                    if (duration > 0)
                    {
                        loader.RunUntil = runUntil;
                    }

                    if (iterations > 0)
                    {
                        loader.NumberOfIterations = iterations;
                    }

                    // Start new thread
                    CMSThread newThread = new CMSThread(loader.Run);
                    newThread.Start();
                }

                return true;
            }
        }

        return false;
    }


    private static void ResetSettings()
    {
        var s = Settings;

        s.LastError = "";
        s.Cancel = false;
        s.SuccessRequests = 0;
        s.Errors = 0;
        s.Run = true;
    }


    private void LoadSettingsFromUI()
    {
        var s = Settings;

        s.Duration = txtDuration.Text.Trim();
        s.Interval = txtInterval.Text.Trim();
        s.Iterations = txtIterations.Text.Trim();
        s.Threads = txtThreads.Text.Trim();
        s.URLs = txtURLs.Text.Trim();
        s.UserAgent = txtUserAgent.Text.Trim();
        s.UserName = ValidationHelper.GetString(userElem.Value, "");
        s.SplitURLs = chkSplitUrls.Checked;
    }


    private void SetEnabled(bool enabled)
    {
        txtDuration.Enabled = enabled;
        txtInterval.Enabled = enabled;
        txtIterations.Enabled = enabled;
        txtThreads.Enabled = enabled;
        txtURLs.Enabled = enabled;
        txtUserAgent.Enabled = enabled;
        userElem.Enabled = enabled;
        chkSplitUrls.Enabled = enabled;
        btnStart.Enabled = enabled;
    }
}