﻿using System;
using System.Configuration;

namespace Rabscuttle.util {
    public class Logger {
        private static readonly object LOG_LOCK = new object();
        private static Category _loggingLevel;

        public enum Category {
            DEBUG,
            INFO,
            WARN,
            ERROR,
            FATAL
        }

        public static void Setup() {
            string logLevel = ConfigurationManager.AppSettings["loglevel"];
            Enum.TryParse(logLevel, true, out _loggingLevel);
        }

        public static void WriteLine(Category category, string component, string format, params object[] args) {
            if (category < _loggingLevel) {
                return;
            }

            string categoryString = ("[" + category.ToString() + "]").PadRight(7);
            string logLine = $"{DateTime.Now.ToLongTimeString()} {categoryString} {component}: {string.Format(format, args)}";

            lock (LOG_LOCK) {
                switch (category) {
                    case Category.DEBUG:
                        Console.ForegroundColor = ConsoleColor.Green;
                        break;
                    case Category.WARN:
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        break;
                    case Category.ERROR:
                        Console.ForegroundColor = ConsoleColor.Red;
                        break;
                    case Category.FATAL:
                        Console.BackgroundColor = ConsoleColor.DarkRed;
                        break;
                }


                if (category == Category.ERROR) {
                    Console.ForegroundColor = ConsoleColor.Red;
                } else if (category == Category.DEBUG) {
                    Console.ForegroundColor = ConsoleColor.Green;
                } else if (category == Category.FATAL) {
                    Console.BackgroundColor = ConsoleColor.DarkRed;
                }
                Console.WriteLine(logLine);
                Console.ResetColor();
            }
        }

        public static void WriteDebug(string component, string format, params object[] args) {
            WriteLine(Category.DEBUG, component, format, args);
        }

        public static void WriteInfo(string component, string format, params object[] args) {
            WriteLine(Category.INFO, component, format, args);
        }

        public static void WriteWarn(string component, string format, params object[] args) {
            WriteLine(Category.WARN, component, format, args);
        }

        public static void WriteError(string component, string format, params object[] args) {
            WriteLine(Category.ERROR, component, format, args);
        }

        public static void WriteFatal(string component, string format, params object[] args) {
            WriteLine(Category.FATAL, component, format, args);
        }
    }
}
