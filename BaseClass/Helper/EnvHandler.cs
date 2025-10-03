using BaseClass.Base.Interface;
using BaseClass.Model;
using BaseLogger;
using BaseLogger.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseClass.Helper
{
    public class EnvHandler
    {
        private readonly IBaseProvider baseConfig;
        private EnvFileHandler _envFileHandler;
        private ILogger? _logWriter;
        public bool _ConfigRead = false;

        public EnvHandler(IBaseProvider baseProvider)
        {
            baseConfig = baseProvider;

            _logWriter = baseProvider.GetItem<ILogger>();

            _envFileHandler = baseProvider.GetItem<EnvFileHandler>();
            //_envFileHandler = new(baseProvider);
            //baseSettings.EnvFileHandler = _envFileHandler;
        }

        public string? EnvRead(string path, EnvAccessMode? mode = null, string? envpath = null, string? envkeyname = null)
        {
            string? data = null;

            try
            {
                if (string.IsNullOrEmpty(path))
                {
                    _logWriter?.LogBase("Was not able to obtain value from given path.");
                    _logWriter?.LogDebug($"Was not able to obtain value from given path. Submitted path => {path}");
                    return null;
                }

                if (mode == null || mode == EnvAccessMode.Project)
                {
                    // Read environment variable from the current process scope
                    data = Environment.GetEnvironmentVariable(path);
                }
                else if (mode == EnvAccessMode.File)
                {
                    data = _envFileHandler?.EnvFileRead(envpath, path, envkeyname);
                }
                else if (mode == EnvAccessMode.User)
                {
                    data = Environment.GetEnvironmentVariable(path, EnvironmentVariableTarget.User);
                }
                else if (mode == EnvAccessMode.System)
                {
                    data = Environment.GetEnvironmentVariable(path, EnvironmentVariableTarget.Machine);
                }

                if (data == null)
                {
                    _logWriter?.LogError($"Unable to obtain value from given path => {path}.");

                    return null;
                }
                else
                {
                    _logWriter?.LogInfo($"Obtained following value {data} from given path => {path}.");

                    return data;
                }
            }
            catch (Exception ex)
            {
                _logWriter?.LogError($"Exception Occured. Exception:{ex.InnerException}; Stack: {ex.StackTrace}; Message: {ex.Message}; Data: {ex.Data}; Source: {ex.Source}");
                return null;
            }
        }

        public void EnvSave(string path, string? data = null, EnvAccessMode? mode = null, string? envpath = null, string? envkeyname = null)
        {
            try
            {
                if (string.IsNullOrEmpty(path) || string.IsNullOrEmpty(data))
                {
                    _logWriter?.LogAlert($"Was not able to obtain value from given path.");
                    _logWriter?.LogDebug($"Was not able to obtain value from given path. Submitted path => {path}");
                    return;
                }

                if (mode == null || mode == EnvAccessMode.Project)
                {
                    // Read environment variable from the current process scope
                    Environment.SetEnvironmentVariable(path, data);
                }
                else if (mode == EnvAccessMode.File)
                {
                    baseConfig.GetItem<IBaseSettings>().FilePath = envpath;
                    _envFileHandler.EnvFileSave(envpath, path, envkeyname, data);
                }
                else if (mode == EnvAccessMode.User)
                {
                    Environment.SetEnvironmentVariable(path, data, EnvironmentVariableTarget.User);
                }
                else if (mode == EnvAccessMode.System)
                {
                    Environment.SetEnvironmentVariable(path, data, EnvironmentVariableTarget.Machine);
                }

                _logWriter?.LogDebug($"Saved following value {data} from given path => {path}.");
            }
            catch (Exception ex)
            {
                _logWriter?.LogError($"Exception Occured. Exception:{ex.InnerException}; Stack: {ex.StackTrace}; Message: {ex.Message}; Data: {ex.Data}; Source: {ex.Source}");
                return;
            }
        }
    }
}
