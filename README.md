```
    ____  _____           _       __  ____                             
   / __ \/ ___/__________(_)___  / /_/ __ \__  ______  ____  ___  _____
  / / / /\__ \/ ___/ ___/ / __ \/ __/ /_/ / / / / __ \/ __ \/ _ \/ ___/
 / /_/ /___/ / /__/ /  / / /_/ / /_/ _, _/ /_/ / / / / / / /  __/ /    
/_____//____/\___/_/  /_/ .___/\__/_/ |_|\__,_/_/ /_/_/ /_/\___/_/     
                       /_/                                             
```

# DScriptRunner

DScriptRunner - это tray only приложение, предназначенное быть удобной точкой входа для запуска пользовательских PowerShell скриптов.

В запущенном состоянии приложение выглядит как иконка в трее (в нижнем правом углу Рабочего стола, или у правого края Панели задач, где отображаются часы и значки некоторых рабочих приложений). По нажатию правой кнопки мыши на значке приложения появляется контекстное меню, позволяющее:
- запустить пользовательские скрипты
- указать путь к файлу конфигурации
- перезагрузить конфигурацию
- закрыть приложение

# Технологии 

- Основа проекта - .Net Core 3.1
- Всплывающие уведомления - Microsoft.Toolkit.Uwp.Notifications v7.1.2
- пользовательские скрипты - PowerShell