```
    ____  _____           _       __  ____                             
   / __ \/ ___/__________(_)___  / /_/ __ \__  ______  ____  ___  _____
  / / / /\__ \/ ___/ ___/ / __ \/ __/ /_/ / / / / __ \/ __ \/ _ \/ ___/
 / /_/ /___/ / /__/ /  / / /_/ / /_/ _, _/ /_/ / / / / / / /  __/ /    
/_____//____/\___/_/  /_/ .___/\__/_/ |_|\__,_/_/ /_/_/ /_/\___/_/     v1.2.0
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

- Основа проекта - `.Net Core 3.1`
- Всплывающие уведомления - `Microsoft.Toolkit.Uwp.Notifications v7.1.2`
- Пользовательские скрипты - `PowerShell`

# Конфигурация

Конфигурирование приложения происходит путем модификации файла ScriptsConfig.xml, который при сборке приложения копируется в результирующую папку.

Основной тег конфигурации `Config` является обязательным и содержит следующие теги:
- `Hello` - сообщение, выдаваемое приложением при завершении загрузки конфигурации _(пустое сообщение при отсутствии)_;
- `Environments`  - **не обязательный**, отвечает за настройку набора сред:
  - `Environment`  - отвечает за настройку конкретной среды:
    - `Before` - **не обязательный**, часть, которая будет добавлена в начало использующего среду скрипта _(например, глобальные переменные; при отсутствии ничего не добавляется)_;
    - `After` - **не обязательный**, часть, которая будет добавлена в конец использующего среду скрипта _(например, пауза после исполнения; при отсутствии ничего не добавляется)_;
- `Scripts` - **обязательный**, отвечает за пользовательские скрипты:
  - `Script` - непосредственно содержит пользовательский код скрипта PowerShell;
  - `Group` - служит для группировки скриптов в подменю для удобства восприятия;

Теги `Script`, `Group` и `Environment` **должны** иметь атрибут `name` - название скрипта/группы в контекстном меню.

# Пример конфигурации

При использовании стандартного файла конфигурации приложение сработает следующим образом::

```xml
<?xml version="1.0" encoding="utf-8" ?>
<Config>
    <Hello>
        Привет! Я готов к работе.
    </Hello>
    <Environments>
        <Environment name="test">
            <Before>
                Write-Host "START"
            </Before>
            <After>
                Write-Host "END"
                pause
            </After>
        </Environment>
        <Environment name="pause-after-end">
            <After>
                pause
            </After>
        </Environment>
    </Environments>
    <Scripts>
        <Group name="Test1">
            <Group name="Test2">
                <Script name="Test2-Script">
                    Write-Host "TEST 3"
                    pause
                </Script>
            </Group>
            <Script name="Test1-Script" env="pause-after-end">
                Write-Host "TEST 2"
            </Script>
        </Group>
        <Script name="Test-Script" env="test">
            Write-Host "TEST 1"
        </Script>
    </Scripts>
</Config>

```

При окончании загрузки конфигурации будет выводиться сообщение "`Привет! Я готов к работе.`"

При этом в контекстном меню появятся:
- Группа `Test1`, содержащая:
    - Группу `Test2`, содержащую:
        - Скрипт `Test2-Script` с содержанием:
            ```powershell
            Write-Host "TEST 3"
            pause
            ```
    - Скрипт `Test1-Script` с содержанием:
        ```powershell
        Write-Host "TEST 2"
        pause
        ```
- Скрипт `Test-Script` с содержанием:
    ```powershell
    Write-Host "START"
    Write-Host "TEST 1"
    Write-Host "END"
    pause
    ```

При нажатии левой кнопкой мыши на одном из скриптов:
- Создается файл tempX.ps1, в который записывается полный код скрипта
- Асинхронно запускается процесс PowerShell, принимающий в качестве аргумента созданный файл
- Ожидается завершение исполнения скрипта
- Созданный файл удаляется