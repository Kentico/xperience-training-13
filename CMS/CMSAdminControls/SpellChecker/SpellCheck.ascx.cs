using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.Caching;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Core;
using CMS.DocumentEngine;
using CMS.Helpers;
using CMS.IO;
using CMS.Localization;
using CMS.MacroEngine;
using CMS.UIControls;

using NetSpell.SpellChecker;
using NetSpell.SpellChecker.Dictionary;


public partial class CMSAdminControls_SpellChecker_SpellCheck : CMSUserControl
{
    #region "Variables"

    private bool mDictExists;
    private string wordCounterString;
    private Spelling SpellChecker;
    private WordDictionary WordDictionary;
    private List<MacroProcessingContext> mMacroIndexes;

    #endregion


    #region "Properties"

    private IEnumerable<MacroProcessingContext> MacroIndexes
    {
        get
        {
            return mMacroIndexes ?? (mMacroIndexes = GetMacroIndexes(CurrentText.Value));
        }
    }

    #endregion


    #region "Page Events"

    protected void Page_Init(object sender, EventArgs e)
    {
        string cacheName = "WordDictionary|" + LocalizationContext.PreferredCultureCode;

        // Get dictionary from cache
        WordDictionary = (WordDictionary)CacheHelper.GetItem(cacheName);
        if (WordDictionary == null)
        {
            // If not in cache, create new
            WordDictionary = new WordDictionary();
            WordDictionary.EnableUserFile = false;

            // Getting folder for dictionaries
            string folderName = "~/App_Data/Dictionaries";
            folderName = MapPath(folderName);

            string culture = LocalizationContext.PreferredCultureCode;
            string dictFile = culture + ".dic";
            string dictPath = Path.Combine(folderName, dictFile);
            if (!File.Exists(dictPath))
            {
                // Get default culture
                string defaultCulture = ValidationHelper.GetString(Service.Resolve<IAppSettingsService>()["CMSDefaultSpellCheckerCulture"], String.Empty);
                if (!String.IsNullOrEmpty(defaultCulture))
                {
                    culture = defaultCulture;
                    dictFile = defaultCulture + ".dic";
                    dictPath = Path.Combine(folderName, dictFile);
                }
            }

            if (!File.Exists(dictPath))
            {
                lblError.Text = string.Format(GetString("SpellCheck.DictionaryNotExists"), culture);
                lblError.Visible = true;
                return;
            }

            mDictExists = true;
            WordDictionary.DictionaryFolder = folderName;
            WordDictionary.DictionaryFile = dictFile;

            // Load and initialize the dictionary
            WordDictionary.Initialize();

            // Store the Dictionary in cache
            var path = Path.Combine(folderName, WordDictionary.DictionaryFile);
            var cd = CacheHelper.GetFileCacheDependency(path);

            CacheHelper.Add(cacheName, WordDictionary, cd, Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration);
        }
        else
        {
            mDictExists = true;
        }

        // Create spell checker
        SpellChecker = new Spelling();
        SpellChecker.ShowDialog = false;
        SpellChecker.Dictionary = WordDictionary;
        SpellChecker.IgnoreAllCapsWords = false;

        // Adding events
        SpellChecker.MisspelledWord += SpellChecker_MisspelledWord;
        SpellChecker.EndOfText += SpellChecker_EndOfText;
        SpellChecker.DoubledWord += SpellChecker_DoubledWord;
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        wordCounterString = GetString("SpellCheck.wordCounterString");

        ScriptHelper.RegisterWOpenerScript(Page);
        ScriptHelper.RegisterScriptFile(Page, "Controls/MessagesPlaceHolder.js");
        ScriptHelper.RegisterSpellChecker(Page);

        // Control initialization
        IgnoreButton.Text = GetString("SpellCheck.IgnoreButton");
        IgnoreAllButton.Text = GetString("SpellCheck.IgnoreAllButton");
        ReplaceButton.Text = GetString("SpellCheck.ReplaceButton");
        ReplaceAllButton.Text = GetString("SpellCheck.ReplaceAllButton");
        StatusText.Text = GetString("SpellCheck.StatusText");
        RemoveButton.Text = GetString("general.remove");

        headTitle.ResourceString = "SpellCheck.lblNotInDictionary";
        lblChangeTo.Text = GetString("SpellCheck.lblChangeTo");
        lblSuggestions.Text = GetString("SpellCheck.lblSuggestions");

        if (mDictExists)
        {
            // Add client side events
            Suggestions.Attributes.Add("onChange", "javascript: changeWord(this);");

            // Load spell checker settings
            LoadValues();
            switch (SpellMode.Value)
            {
                case "start":
                    EnableButtons();
                    SpellChecker.SpellCheck();
                    break;

                case "suggest":
                    EnableButtons();
                    break;

                default:
                    DisableButtons();
                    break;
            }
        }
    }


    protected void Page_PreRender(object sender, EventArgs e)
    {
        // Set browser class
#pragma warning disable CS0618 // Type or member is obsolete
        SpellingBody.Attributes.Add("class", BrowserHelper.GetBrowserClass());
#pragma warning restore CS0618 // Type or member is obsolete

        // Register the scripts
        string script = String.Format("initialize(\"{0}\", \"{1}\", \"{2}\", \"{3}\", \"{4}\", \"{5}\", \"{6}\", \"{7}\");", 
            WordIndex.ClientID, 
            CurrentText.ClientID, 
            ElementIndex.ClientID, 
            SpellMode.ClientID, 
            StatusText.ClientID, 
            ReplacementWord.ClientID, 
            ResHelper.GetString("spellcheck.statustextcheck"), 
            ResHelper.GetString("spellcheck.statustextfail"));
        ScriptHelper.RegisterStartupScript(this, typeof(string), "Init_" + ClientID, ScriptHelper.GetScript(script));
    }

    #endregion


    #region "Methods"

    protected void EnableButtons()
    {
        IgnoreButton.Enabled = true;
        IgnoreAllButton.Enabled = true;
        btnAdd.Enabled = true;
        ReplaceButton.Enabled = true;
        ReplaceAllButton.Enabled = true;
        ReplacementWord.Enabled = true;
        Suggestions.Enabled = true;
        RemoveButton.Enabled = true;
    }


    protected void DisableButtons()
    {
        IgnoreButton.Enabled = false;
        IgnoreAllButton.Enabled = false;
        btnAdd.Enabled = false;
        ReplaceButton.Enabled = false;
        ReplaceAllButton.Enabled = false;
        ReplacementWord.Enabled = false;
        Suggestions.Enabled = false;
        RemoveButton.Enabled = false;
    }


    protected void SaveValues()
    {
        CurrentText.Value = SpellChecker.Text;
        mMacroIndexes = null;

        WordIndex.Value = SpellChecker.WordIndex.ToString();

        // Save ignore words
        string[] ignore = (string[])SpellChecker.IgnoreList.ToArray(typeof(string));

        IgnoreList.Value = String.Join("|", ignore);

        // Save replace words
        ArrayList tempArray = new ArrayList(SpellChecker.ReplaceList.Keys);
        string[] replaceKey = (string[])tempArray.ToArray(typeof(string));

        ReplaceKeyList.Value = String.Join("|", replaceKey);
        tempArray = new ArrayList(SpellChecker.ReplaceList.Values);

        string[] replaceValue = (string[])tempArray.ToArray(typeof(string));

        ReplaceValueList.Value = String.Join("|", replaceValue);

        // Saving user words
        tempArray = new ArrayList(SpellChecker.Dictionary.UserWords.Keys);

        string[] userWords = (string[])tempArray.ToArray(typeof(string));

        CookieHelper.SetValue(CookieName.SpellCheckUserWords, String.Join("|", userWords), "/", DateTime.Now.AddMonths(1));
    }


    protected void LoadValues()
    {
        if (CurrentText.Value.Length > 0)
        {
            SpellChecker.Text = CurrentText.Value;
        }

        pnlCurrentWord.Visible = CurrentText.Value.Length > 0;

        if (WordIndex.Value.Length > 0)
        {
            SpellChecker.WordIndex = int.Parse(WordIndex.Value);
        }

        // Restore ignore list
        if (IgnoreList.Value.Length > 0)
        {
            SpellChecker.IgnoreList.Clear();
            SpellChecker.IgnoreList.AddRange(IgnoreList.Value.Split('|'));
        }

        LoadIgnoreList();

        // Restore replace list
        if (ReplaceKeyList.Value.Length > 0 && ReplaceValueList.Value.Length > 0)
        {
            string[] replaceKeys = ReplaceKeyList.Value.Split('|');
            string[] replaceValues = ReplaceValueList.Value.Split('|');

            SpellChecker.ReplaceList.Clear();
            if (replaceKeys.Length == replaceValues.Length)
            {
                for (int i = 0; i < replaceKeys.Length; i++)
                {
                    if (replaceKeys[i].Length > 0)
                    {
                        SpellChecker.ReplaceList.Add(replaceKeys[i], replaceValues[i]);
                    }
                }
            }
        }

        // Restore user words
        SpellChecker.Dictionary.UserWords.Clear();
        if (CookieHelper.RequestCookieExists(CookieName.SpellCheckUserWords))
        {
            string[] userWords = CookieHelper.GetValue(CookieName.SpellCheckUserWords).Split('|');

            foreach (string t in userWords)
            {
                if (t.Length > 0)
                {
                    SpellChecker.Dictionary.UserWords.Add(t, t);
                }
            }
        }
    }

    #endregion


    #region "Event Handlers"

    /// <summary>
    /// Loads Ignore list with special characters which should not be checked by spell checker.
    /// </summary>
    protected void LoadIgnoreList()
    {
        const string ignoreList = "quot ETH Ntilde Ograve Oacute Ocirc Otilde Ouml times Oslash Ugrave Uacute Ucirc Uuml Yacute THORN szlig agrave aacute acirc atilde auml aring aelig ccedil egrave eacute ecirc euml igrave iacute icirc iuml eth ntilde ograve oacute ocirc otilde ouml divide oslash ugrave uacute ucirc uuml yacute thorn yuml OElig oelig Scaron scaron Yuml fnof circ tilde Alpha Beta Gamma Delta Epsilon Zeta Eta Theta Iota Kappa Lambda Mu Nu Xi Omicron Pi Rho Sigma Tau Upsilon Phi Chi Psi Omega alpha beta gamma delta epsilon zeta eta theta iota kappa lambda mu nu xi omicron pi rho sigmaf sigma tau upsilon phi chi psi omega thetasym upsih piv ensp emsp thinsp zwnj zwj lrm rlm ndash mdash lsquo rsquo sbquo ldquo rdquo bdquo dagger Dagger bull hellip permil prime Prime lsaquo rsaquo oline frasl euro image weierp real trade alefsym larr uarr rarr darr harr crarr lArr uArr rArr dArr hArr forall part exist empty nabla isin notin ni prod sum minus lowast radic prop infin ang and or cap cup int there4 sim cong asymp ne equiv le ge sub sup nsub sube supe oplus otimes perp sdot lceil rceil lfloor rfloor lang rang loz spades clubs hearts diams quot apos amp gt lt nbsp iexcl iquest raquo laquo cent copy micro middot para plusmn pound reg sect yen cedil macr uml curren brvbar ordf ordm sup1 frac12 frac14 frac34 sup2 sup3";

        var ignoreArr = ignoreList.Split(new[] { ' ' });

        foreach (string ingoreItem in ignoreArr)
        {
            SpellChecker.IgnoreList.Add(ingoreItem);
        }
    }


    protected void SpellChecker_DoubledWord(object sender, SpellingEventArgs e)
    {
        SaveValues();
        headTitle.ResourceString = "SpellCheck.DoubledWord";
        CurrentWord.Text = SpellChecker.CurrentWord;
        Suggestions.Items.Clear();
        ReplacementWord.Text = string.Empty;
        SpellMode.Value = "suggest";
        StatusText.Text = string.Format(wordCounterString, SpellChecker.WordIndex + 1, SpellChecker.WordCount);
        btnAdd.Enabled = false;
    }


    protected void SpellChecker_EndOfText(object sender, EventArgs e)
    {
        SaveValues();
        SpellMode.Value = "end";
        DisableButtons();
        ShowConfirmation(GetString("SpellCheck.SpellComplete"));
        StatusText.Text = string.Format(wordCounterString, SpellChecker.WordIndex + 1, SpellChecker.WordCount);
    }


    protected void SpellChecker_MisspelledWord(object sender, SpellingEventArgs e)
    {
        // Ignore macro content
        if (IsInMacro(e.TextIndex))
        {
            SpellChecker.IgnoreWord();
            SpellChecker.SpellCheck();

            return;
        }

        SaveValues();
        CurrentWord.Text = SpellChecker.CurrentWord;
        SpellChecker.Suggest();

        Suggestions.DataSource = SpellChecker.Suggestions;
        Suggestions.DataBind();

        if (Suggestions.Items.Count > 0)
        {
            Suggestions.SelectedIndex = 0;
        }

        ReplacementWord.Text = Suggestions.SelectedValue;
        SpellMode.Value = "suggest";
        StatusText.Text = string.Format(wordCounterString, SpellChecker.WordIndex + 1, SpellChecker.WordCount);
    }


    /// <summary>
    /// Returns true if the given index is present in macro
    /// </summary>
    /// <param name="index">Index to check</param>
    private bool IsInMacro(int index)
    {
        return MacroIndexes.Any(context => (context.MacroStart <= index) && (context.MacroEnd >= index));
    }


    private static List<MacroProcessingContext> GetMacroIndexes(string text)
    {
        var macroIndexes = new List<MacroProcessingContext>();

        MacroProcessor.ProcessDataMacros(text, MacroProcessor.NOT_RESOLVE, context =>
        {
            macroIndexes.Add(context);
            return null;
        });
        return macroIndexes;
    }


    protected void IgnoreButton_Click(object sender, EventArgs e)
    {
        SpellChecker.IgnoreWord();
        SpellChecker.SpellCheck();
    }


    protected void IgnoreAllButton_Click(object sender, EventArgs e)
    {
        SpellChecker.IgnoreAllWord();
        SpellChecker.SpellCheck();
    }


    protected void AddButton_Click(object sender, EventArgs e)
    {
        if (!SpellChecker.Dictionary.Contains(SpellChecker.CurrentWord))
        {
            SpellChecker.Dictionary.Add(SpellChecker.CurrentWord);
            SpellChecker.SpellCheck();
        }
    }


    protected void RemoveButton_Click(object sender, EventArgs e)
    {
        SpellChecker.ReplaceWord(String.Empty);
        CurrentText.Value = SpellChecker.Text;
        SpellChecker.SpellCheck();
    }


    protected void ReplaceButton_Click(object sender, EventArgs e)
    {
        SpellChecker.ReplaceWord(ReplacementWord.Text);
        CurrentText.Value = SpellChecker.Text;
        SpellChecker.SpellCheck();
    }


    protected void ReplaceAllButton_Click(object sender, EventArgs e)
    {
        SpellChecker.ReplaceAllWord(ReplacementWord.Text);
        CurrentText.Value = SpellChecker.Text;
        SpellChecker.SpellCheck();
    }

    #endregion
}
