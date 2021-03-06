﻿using System;
using System.Collections.Generic;
using System.Security;
using System.Text;
using System.Threading;
using Extension;
using EnsoPlus.Entities;

namespace EnsoPlus
{
    public class SuggestionsCache
    {
        private static Dictionary<string, Dictionary<string, IWorkItem>> suggestionsByType = new Dictionary<string, Dictionary<string, IWorkItem>>();

	    public static void BuildAllCache()
	    {
			Thread thread = new Thread(BuildAllCacheInNewThread);
			thread.Start();    
	    }

	    private static void BuildAllCacheInNewThread()
	    {
		    Get(new WorkItemsProviders.BackupProfiles.BackupProfiles());
		    Get(new WorkItemsProviders.BookmarkBrowser.BookmarkBrowser());
		    Get(new WorkItemsProviders.CallerHistory.CallerHistory());
		    Get(new WorkItemsProviders.Clipboard.ClipboardImage());
		    Get(new WorkItemsProviders.Clipboard.ClipboardText());
		    Get(new WorkItemsProviders.CommandsHistory.CommandsHistory());
		    Get(new WorkItemsProviders.Contacts.Contacts());
		    Get(new WorkItemsProviders.DisplayMessagesHistory.DisplayMessagesHistory());
		    Get(new WorkItemsProviders.Macros.Macros());
		    Get(new WorkItemsProviders.MemorizedData.MemorizedData());
		    Get(new WorkItemsProviders.Misc.AutoGeneratedText());
		    Get(new WorkItemsProviders.Processes.Processes());
		    Get(new WorkItemsProviders.ReflectionData.ReflectionData());
		    Get(new WorkItemsProviders.Shortcuts.FilesWorkItems());
		    Get(new WorkItemsProviders.Shortcuts.Shortcuts());
		    Get(new WorkItemsProviders.Shortcuts.ShortcutsLists());
		    Get(new WorkItemsProviders.Shortcuts.StartMenuShortcuts());
		    Get(new WorkItemsProviders.ShortcutTemplates.ShortcutTemplates());
	    }

		private static object _syncObject = new object();
        public static Dictionary<string, IWorkItem> Get(IWorkItemsProvider ptp)
        {
            Dictionary<string, IWorkItem> suggestions = null;

            if(!ptp.SuggestionsCachingAllowed())
            {
                suggestions = ptp.GetSuggestions();
            }else
            {
	            lock (_syncObject)
	            {
		            if (!suggestionsByType.TryGetValue(ptp.GetType().FullName, out suggestions))
		            {
			            suggestions = ptp.GetSuggestions();

			            suggestionsByType.Add(ptp.GetType().FullName, suggestions);
		            }
	            }
            }

            return suggestions;
        }

        public static void DropCache(Type parameterTypeProviderType)
        {
            if (suggestionsByType.ContainsKey(parameterTypeProviderType.FullName))
            {
                suggestionsByType.Remove(parameterTypeProviderType.FullName);
            }
        }

        public static void DropAllCache()
        {
            suggestionsByType.Clear();
        }
    }
}
