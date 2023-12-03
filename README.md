```
    ____  _____           _       __  ____                             
   / __ \/ ___/__________(_)___  / /_/ __ \__  ______  ____  ___  _____
  / / / /\__ \/ ___/ ___/ / __ \/ __/ /_/ / / / / __ \/ __ \/ _ \/ ___/
 / /_/ /___/ / /__/ /  / / /_/ / /_/ _, _/ /_/ / / / / / / /  __/ /    
/_____//____/\___/_/  /_/ .___/\__/_/ |_|\__,_/_/ /_/_/ /_/\___/_/     v2.0.0
                       /_/                                             
```

# DScriptRunner

***DScriptRunner*** - это tray only приложение, предназначенное быть удобной точкой входа для запуска пользовательских PowerShell скриптов.

В запущенном состоянии приложение выглядит как иконка в трее (в нижнем правом углу Рабочего стола, или у правого края Панели задач, где отображаются часы и значки некоторых рабочих приложений). По нажатию правой кнопки мыши на значке приложения появляется контекстное меню, позволяющее:
- запустить пользовательские скрипты
- найти файл конфигурации
- перезагрузить конфигурацию
- закрыть приложение

# Технологии 

- Основа проекта - `.Net Core 6.0`
- Пользовательские скрипты - `PowerShell`

# Конфигурация

Конфигурирование приложения происходит путем модификации файла ScriptsConfig.xml, который при сборке приложения копируется в результирующую папку.

Основной тег конфигурации `Config` является обязательным и содержит следующие теги:
- `Hello` - сообщение, выдаваемое приложением при завершении загрузки конфигурации _(пустое сообщение при отсутствии)_;
- `Environments`  - **не обязательный**, отвечает за настройку набора сред:
  - `Environment`  - отвечает за настройку конкретной среды (дожна быть хотя бы одна вложенная секция, описывающая среду):
    - `Before` - **не обязательный**, часть, которая будет добавлена в начало использующего среду скрипта _(например, глобальные переменные; при отсутствии ничего не добавляется)_;
    - `After` - **не обязательный**, часть, которая будет добавлена в конец использующего среду скрипта _(например, пауза после исполнения; при отсутствии ничего не добавляется)_;
- `ScriptInserts` - **не обязательный**, отвечает за настройку скриптовых вставок:
  - `ScriptInsert` - код, который будет добавлен в скрипт _(например, глобальные переменные)_;
- `Scripts` - **обязательный**, отвечает за пользовательские скрипты:
  - `Group` - служит для группировки скриптов в подменю для удобства восприятия;
  - `Script` - кнопка запуска пользовательского скрипта (должен содержать код PowerShell или хотя бы один вложенный тег `ScriptText` или `ScriptInsert`);
    - `ScriptText` - **не обязательный**, непосредственно содержит пользовательский код скрипта PowerShell;
    - `ScriptInsert` - **не обязательный**, вставка описанного ранее скрипта PowerShell;

Теги `Script`, `Group`, `ScriptInsert` и `Environment` **должны** иметь атрибут `name` - название скрипта или группы в контекстном меню, вставки или среды. 
Теги в секции `Scripts` могут иметь атрибут `env` - название используемой среды.

# Пример конфигурации

При использовании стандартного файла конфигурации приложение сработает следующим образом:

```xml
<?xml version="1.0" encoding="utf-8" ?>
<Config>
    <Hello>
        Привет! Я готов к работе.
    </Hello>

    <Environments>
        <Environment name="test-text">
            <Before>
                Write-Host "ENV BEFORE"
            </Before>
            <After>
                Write-Host "ENV AFTER"
            </After>
        </Environment>
        <Environment name="test-text-2">
            <Before>
                Write-Host "ENV BEFORE 2"
            </Before>
            <After>
                Write-Host "ENV AFTER 2"
            </After>
        </Environment>
        <Environment name="pause-after-end">
            <After>
                pause
            </After>
        </Environment>
    </Environments>

    <ScriptInserts>
        <ScriptInsert name="start-text">
            Write-Host "START"
        </ScriptInsert>
        <ScriptInsert name="end-text">
            Write-Host "END"
        </ScriptInsert>
        <ScriptInsert name="pause-after-end">
            pause
        </ScriptInsert>
    </ScriptInserts>

    <Scripts>
        <Group name="Test-Group">
            <Group name="Test-Inner-Group">
                <Script name="Test-Inner-Group-Script">
                    <ScriptText>
                        Write-Host "TEST Inner-Group-Script"
                        pause
                    </ScriptText>
                </Script>
            </Group>
            <Script name="Test-Group-Script">
                <ScriptText>
                    Write-Host "TEST Group-Script"
                    pause
                </ScriptText>
            </Script>
        </Group>

        <Script name="Test-Script">
            Write-Host "TEST script"
            pause
        </Script>

        <Script name="Test-Script-2">
            <ScriptText>
                Write-Host "TEST script 2"
                pause
            </ScriptText>
        </Script>

        <Script name="Test-HiddenScript" hidden="">
            <ScriptText>
                Write-Host "TEST hidden"
                pause
            </ScriptText>
        </Script>

        <Script name="Test-Inserts">
            <ScriptInsert name="start-text" />
            <ScriptText>
                Write-Host "TEST Inserts"
            </ScriptText>
            <ScriptInsert name="end-text" />
            <ScriptText>
                pause
            </ScriptText>
        </Script>

        <Group name="Test-Env-Group" env="pause-after-end">
            <Group name="Test-Env-Inner-Group" env="test-text">
                <Script name="Test-Env-Inner-Group-Script" env="test-text-2">
                    <ScriptText>
                        Write-Host "TEST Inner-Group-Script"
                    </ScriptText>
                </Script>
            </Group>
            <Script name="Test-Env-Group-Script" env="test-text">
                <ScriptText>
                    Write-Host "TEST Group-Script"
                </ScriptText>
            </Script>
        </Group>

        <Script name="Test-Environments" env="pause-after-end">
            <ScriptText env="test-text">
                Write-Host "TEST Environments"
            </ScriptText>
            <ScriptInsert name="end-text" env="test-text" />
        </Script>
    </Scripts>
</Config>
```

При окончании загрузки конфигурации будет выводиться сообщение "`Привет! Я готов к работе.`"

При этом в контекстном меню появятся:
- Группа `Test-Group`, содержащая:
    - Группу `Test-Inner-Group`, содержащую:
        - Скрипт `Test-Inner-Group-Script` с содержанием:
            ```powershell
            Write-Host "TEST Inner-Group-Script"
            pause
            ```
    - Скрипт `Test-Group-Script` с содержанием:
        ```powershell
        Write-Host "TEST Group-Script"
        pause
        ```
- Скрипт `Test-Script` с содержанием:
    ```powershell
    Write-Host "TEST script"
    pause
    ```
- Скрипт `Test-Script-2` с содержанием:
    ```powershell
    Write-Host "TEST script 2"
    pause
    ```
- Скрипт `Test-HiddenScript` не будет отображаться и обрабатываться, т.к. он скрыт
- Скрипт `Test-Inserts` с содержанием:
    ```powershell
    Write-Host "START"
    Write-Host "TEST Inserts"
    Write-Host "END"
    pause
    ```
- Группа `Test-Env-Group`, содержащая:
    - Группу `Test-Env-Inner-Group`, содержащую:
        - Скрипт `Test-Env-Inner-Group-Script` с содержанием:
            ```powershell
            Write-Host "ENV BEFORE"
            Write-Host "ENV BEFORE 2"
            Write-Host "TEST Inner-Group-Script"
            Write-Host "ENV AFTER 2"
            Write-Host "ENV AFTER"
            pause
            ```
    - Скрипт `Test-Env-Group-Script` с содержанием:
        ```powershell
        Write-Host "ENV BEFORE"
        Write-Host "TEST Group-Script"
        Write-Host "ENV AFTER"
        pause
        ```
- Скрипт `Test-Environments` с содержанием:
    ```powershell
    Write-Host "ENV BEFORE"
    Write-Host "TEST Environments"
    Write-Host "ENV AFTER"
    Write-Host "ENV BEFORE"
    Write-Host "END"
    Write-Host "ENV AFTER"
    pause
    ```

При нажатии левой кнопкой мыши на одном из скриптов:
- Создается файл tempX.ps1, в который записывается полный код скрипта
- Асинхронно запускается процесс PowerShell, принимающий в качестве аргумента созданный файл
- Ожидается завершение исполнения скрипта
- Созданный файл удаляется

## Внимание! 

Для корректной работы DScriptRunner нужно изменить политику запуска скриптов PowerShell, разрешив выполнение локальных скриптов. 

Для этого нужно запустить PowerShell в режиме Администратора и выполнить команду 
```powershell
Set-ExecutionPolicy RemoteSigned
```
