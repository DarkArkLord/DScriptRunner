﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан программой.
//     Исполняемая версия:4.0.30319.42000
//
//     Изменения в этом файле могут привести к неправильной работе и будут потеряны в случае
//     повторной генерации кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DScriptRunner.Resources {
    using System;
    
    
    /// <summary>
    ///   Класс ресурса со строгой типизацией для поиска локализованных строк и т.д.
    /// </summary>
    // Этот класс создан автоматически классом StronglyTypedResourceBuilder
    // с помощью такого средства, как ResGen или Visual Studio.
    // Чтобы добавить или удалить член, измените файл .ResX и снова запустите ResGen
    // с параметром /str или перестройте свой проект VS.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class AppResources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal AppResources() {
        }
        
        /// <summary>
        ///   Возвращает кэшированный экземпляр ResourceManager, использованный этим классом.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("DScriptRunner.Resources.AppResources", typeof(AppResources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Перезаписывает свойство CurrentUICulture текущего потока для всех
        ///   обращений к ресурсу с помощью этого класса ресурса со строгой типизацией.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Поиск локализованного ресурса типа System.Drawing.Icon, аналогичного (Значок).
        /// </summary>
        internal static System.Drawing.Icon AppIcon {
            get {
                object obj = ResourceManager.GetObject("AppIcon", resourceCulture);
                return ((System.Drawing.Icon)(obj));
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Конфигурация.
        /// </summary>
        internal static string ButtonConfig {
            get {
                return ResourceManager.GetString("ButtonConfig", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Выход.
        /// </summary>
        internal static string ButtonExit {
            get {
                return ResourceManager.GetString("ButtonExit", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Инфо.
        /// </summary>
        internal static string ButtonInfo {
            get {
                return ResourceManager.GetString("ButtonInfo", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Перезагрузка.
        /// </summary>
        internal static string ButtonRefresh {
            get {
                return ResourceManager.GetString("ButtonRefresh", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на ScriptsConfig.xml.
        /// </summary>
        internal static string ConfigFileName {
            get {
                return ResourceManager.GetString("ConfigFileName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на DScriptRunner: Запущен.
        /// </summary>
        internal static string HelloCaption {
            get {
                return ResourceManager.GetString("HelloCaption", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на О программе.
        /// </summary>
        internal static string InfoCaption {
            get {
                return ResourceManager.GetString("InfoCaption", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на О программе DScriptRunner
        ///Разработчик: Алексей Петров aka DarkArkLord
        ///https://github.com/DarkArkLord/DScriptRunner
        ///2022-2023 год
        ///Версия 2.0.0.
        /// </summary>
        internal static string InfoMessage {
            get {
                return ResourceManager.GetString("InfoMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на DScriptRunner: Ошибка при загрузке конфигурации.
        /// </summary>
        internal static string LoadErrorCaption {
            get {
                return ResourceManager.GetString("LoadErrorCaption", resourceCulture);
            }
        }
    }
}
