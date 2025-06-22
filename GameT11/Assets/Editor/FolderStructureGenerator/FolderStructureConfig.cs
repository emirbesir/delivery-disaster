using System.Collections.Generic;
using UnityEngine;

namespace Editor
{
    [CreateAssetMenu(fileName = "FolderStructureConfig", menuName = "Unity Tools/Folder Structure Config")]
    public class FolderStructureConfig : ScriptableObject
    {
        [Header("Project Settings")]
        [Tooltip("Default project name that will appear in the folder creation window")]
        public string defaultProjectName = "_PROJECT_NAME";
        [Tooltip("Define the main folders and their subfolders")]
        public List<FolderGroup> folderGroups = new List<FolderGroup>();
        [Tooltip("Additional standalone folders to create")]
        public List<string> standaloneFolders = new List<string>();
    
        [Header("Additional Options")]
        [Tooltip("Create .gitkeep files in empty folders for version control")]
        public bool createGitKeepFiles = true;
    
        [System.Serializable]
        public class FolderGroup
        {
            [Tooltip("Name of the main folder")]
            public string mainFolder;
        
            [Tooltip("Subfolders to create inside the main folder")]
            public List<string> subfolders = new List<string>();
        
            [Tooltip("Enable/disable this folder group")]
            public bool enabled = true;
        }
    
        // Method to get the main folder structure as a dictionary
        public Dictionary<string, List<string>> GetMainFolderStructure()
        {
            Dictionary<string, List<string>> structure = new Dictionary<string, List<string>>();
        
            // Add main folders and their subfolders (filter out empty ones)
            foreach (var group in folderGroups)
            {
                if (group.enabled && !string.IsNullOrWhiteSpace(group.mainFolder))
                {
                    List<string> validSubfolders = new List<string>();
                    foreach (string subfolder in group.subfolders)
                    {
                        if (!string.IsNullOrWhiteSpace(subfolder))
                        {
                            validSubfolders.Add(subfolder);
                        }
                    }
                    structure[group.mainFolder] = validSubfolders;
                }
            }
            return structure;
        }
        
        // Method to get the standalone folder structure as a dictionary
        public Dictionary<string, List<string>> GetStandaloneFolderStructure()
        {
            Dictionary<string, List<string>> structure = new Dictionary<string, List<string>>();
        
            // Add standalone folders (filter out empty ones)
            foreach (var folder in standaloneFolders)
            {
                if (!string.IsNullOrWhiteSpace(folder))
                {
                    structure[folder] = new List<string>();
                }
            }
        
            return structure;
        }
    
        private void OnValidate()
        {
            // Only clean up empty entries if we have more than one empty entry
            // This allows users to add new items through the Inspector
        
            // Clean standalone folders - keep at least one empty entry for adding new items
            if (standaloneFolders.Count > 1)
            {
                int emptyCount = 0;
                for (int i = standaloneFolders.Count - 1; i >= 0; i--)
                {
                    if (string.IsNullOrWhiteSpace(standaloneFolders[i]))
                    {
                        emptyCount++;
                        if (emptyCount > 1)
                        {
                            standaloneFolders.RemoveAt(i);
                        }
                    }
                }
            }
        
            // Clean subfolders - keep at least one empty entry for adding new items
            foreach (var group in folderGroups)
            {
                if (group.subfolders != null && group.subfolders.Count > 1)
                {
                    int emptyCount = 0;
                    for (int i = group.subfolders.Count - 1; i >= 0; i--)
                    {
                        if (string.IsNullOrWhiteSpace(group.subfolders[i]))
                        {
                            emptyCount++;
                            if (emptyCount > 1)
                            {
                                group.subfolders.RemoveAt(i);
                            }
                        }
                    }
                }
            }
        }
    }
}