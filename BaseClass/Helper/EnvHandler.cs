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
using FuncName = BaseClass.MethodNameExtractor.FuncNameExtractor;

namespace BaseClass.Helper
{
    public class EnvHandler
    {
        private readonly IBase baseConfig;
        private EnvFileHandler _envFileHandler;
        private LogWriter? _logWriter;
        public bool _ConfigRead = false;

        public EnvHandler(IBase baseSettings)
        {
            baseConfig = baseSettings;

            _logWriter = baseSettings.Logger;

            _envFileHandler = new(baseSettings);
            baseSettings.EnvFileHandler = _envFileHandler;
        }

        public string? EnvRead(string path, EnvAccessMode? mode = null, string? envpath = null, string? envkeyname = null)
        {
            string? data = null;

            try
            {
                if (string.IsNullOrEmpty(path))
                {
                    _logWriter?.LogWrite($"Was not able to obtain value from given path.", this.GetType().Name, FuncName.GetMethodName(), MessageLevels.Verbose);
                    _logWriter?.LogWrite($"Was not able to obtain value from given path. Submitted path => {path}", this.GetType().Name, FuncName.GetMethodName(), MessageLevels.Debug);
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
                    _logWriter?.LogWrite($"Unable to obtain value from given path => {path}.", this.GetType().Name, FuncName.GetMethodName(), MessageLevels.Fatal);

                    return null;
                }
                else
                {
                    _logWriter?.LogWrite($"Obtained following value {data} from given path => {path}.", this.GetType().Name, FuncName.GetMethodName(), MessageLevels.Log);

                    return data;
                }
            }
            catch (Exception ex)
            {
                _logWriter?.LogWrite($"Exception Occured. Exception:{ex.InnerException}; Stack: {ex.StackTrace}; Message: {ex.Message}; Data: {ex.Data}; Source: {ex.Source}", this.GetType().Name, FuncName.GetMethodName(), MessageLevels.Fatal);
                return null;
            }
        }

        public void EnvSave(string path, string? data = null, EnvAccessMode? mode = null, string? envpath = null, string? envkeyname = null)
        {
            try
            {
                if (string.IsNullOrEmpty(path) || string.IsNullOrEmpty(data))
                {
                    _logWriter?.LogWrite($"Was not able to obtain value from given path.", this.GetType().Name, FuncName.GetMethodName(), MessageLevels.Verbose);
                    _logWriter?.LogWrite($"Was not able to obtain value from given path. Submitted path => {path}", this.GetType().Name, FuncName.GetMethodName(), MessageLevels.Debug);
                    return;
                }

                if (mode == null || mode == EnvAccessMode.Project)
                {
                    // Read environment variable from the current process scope
                    Environment.SetEnvironmentVariable(path, data);
                }
                else if (mode == EnvAccessMode.File)
                {
                    baseConfig.FilePath = envpath;
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

                _logWriter?.LogWrite($"Saved following value {data} from given path => {path}.", this.GetType().Name, FuncName.GetMethodName(), MessageLevels.Log);
            }
            catch (Exception ex)
            {
                _logWriter?.LogWrite($"Exception Occured. Exception:{ex.InnerException}; Stack: {ex.StackTrace}; Message: {ex.Message}; Data: {ex.Data}; Source: {ex.Source}", this.GetType().Name, FuncName.GetMethodName(), MessageLevels.Fatal);
                return;
            }
        }
    }
}
