﻿namespace Kudu.Core
{
    public interface IEnvironment
    {
        string RootPath { get; }                // e.g. /
        string SiteRootPath { get; }            // e.g. /site
        string RepositoryPath { get; set; }     // e.g. /site/repository
        string WebRootPath { get; }             // e.g. /site/wwwroot
        string DeploymentsPath { get; }         // e.g. /site/deployments
        string DeploymentToolsPath { get; }     // e.g. /site/deployments/tools
        string DiagnosticsPath { get; }         // e.g. /site/diagnostics
        string LocksPath { get; }               // e.g. /site/locks
        string SSHKeyPath { get; }
        string TempPath { get; }
        string ScriptPath { get; }

        /// <summary>
        /// Path to Kudu SCM site binary folder
        /// </summary>
        string BinPath { get; } 
        string NodeModulesPath { get; }
        string LogFilesPath { get; }            // e.g. /logfiles
        string ApplicationLogFilesPath { get; } // e.g. /logfiles/application
        string TracePath { get; }               // e.g. /logfiles/kudu/trace
        string AnalyticsPath { get; }           // e.g. %temp%/siteExtLogs
        string DeploymentTracePath { get; }     // e.g. /logfiles/kudu/deployment
        string DataPath { get; }                // e.g. /data
        string JobsDataPath { get; }            // e.g. /data/jobs
        string JobsBinariesPath { get; }        // e.g. /site/wwwroot/app_data/jobs
    }
}
