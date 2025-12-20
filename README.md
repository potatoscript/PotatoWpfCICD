# 🎯 Goal of This CI/CD

When you:

1. Push code to GitHub
2. GitHub automatically:

   * Builds your WPF app
   * Runs tests
   * Publishes output
   * Creates a downloadable artifact (ZIP / EXE)

# 🧠 CI/CD for WPF (Mental Model)

| Step     | What happens               |
| -------- | -------------------------- |
| CI       | Build + test automatically |
| CD       | Package app for release    |
| Artifact | Downloadable output        |


# File Structure
```
PotatoWpfApp/
│
├── .github/
│   └── workflows/
│       └── wpf-ci.yml        👈 CI/CD pipeline
│
├── src/PotatoWpfCICD/        👈 WPF app project
│   ├── App.xaml
│   ├── App.xaml.cs
│   ├── MainWindow.xaml
│   ├── MainWindow.xaml.cs
│   ├── ViewModels/
│   │   └── MainViewModel.cs
│   ├── Models/
│   │   └── SampleModel.cs
│   ├── Resources/
│   │   └── Styles.xaml
│   ├── PotatoWpfCICD.csproj
│   └── bin/
│   └── obj/
│
├── PotatoWpfCICD.Tests/       👈 Unit tests
│   ├── UnitTest1.cs
│   └── PotatoWpfCICD.Tests.csproj
│
├── .gitignore
├── PotatoWpfCICD.sln          👈 Solution file
└── README.md
```

# 🧱 Step 1: Setup WPF Project

## 1.1 Create WPF App

```bash
dotnet new wpf -n PotatoWpfCICD
cd PotatoWpfCICD
```

## 1.2 Confirm it builds locally

```bash
dotnet restore
dotnet build -c Release
dotnet test -c Release
dotnet run
```

If this fails locally, CI will fail too.

---

# 🗂 Step 2: Setup GitHub Repository

## 2.1 Initialize Git

```bash
git init
git add .
git commit -m "Initial WPF project"
```

## 2.2 Push to GitHub

```bash
git branch -M main
git remote add origin https://github.com/YOURNAME/PotatoWpfCICD.git
git push -u origin main
```

---

# ⚙️ Step 3: Create GitHub Actions Workflow

GitHub Actions = CI/CD engine.

## 3.1 Folder Structure

Create this:

```text
.github/
 └── workflows/
      └── wpf-ci.yml
```

---

# 🧪 Step 4: Basic CI (Build WPF)

## 4.1 `wpf-ci.yml` (Minimal Build)

```yaml
name: WPF CI

on:
  push:
    branches: [ "main" ]
  pull_request:
    branches: [ "main" ]

jobs:
  build:
    runs-on: windows-latest

    steps:
    - name: Checkout code
      uses: actions/checkout@v4

    - name: Setup .NET
      uses: actions/setup-dotnet@v4
      with:
        dotnet-version: '8.0.x'

    - name: Restore
      run: dotnet restore

    - name: Build
      run: dotnet build --configuration Release
```

✅ This already gives you **CI**.

Push code → GitHub builds your WPF app automatically.

---

# 🧪 Step 5: Add Unit Tests

## 5.1 Create Test Project

```bash
dotnet new xunit -n tests/PotatoWpfCICD.Tests
dotnet add PotatoWpfCICD.Tests reference PotatoWpfCICD.csproj
```

## 5.2 Add Test Step

Update workflow:

```yaml
- name: Test
  run: dotnet test --configuration Release
```

Now CI = **build + test**.

---

# 📦 Step 6: Publish WPF App (CD Part)

## 6.1 Publish Command (Local Test First)

```bash
dotnet publish PotatoWpfCICD.csproj ^
  -c Release ^
  -r win-x64 ^
  --self-contained true ^
  /p:PublishSingleFile=true
```

Output goes to:

```text
bin/Release/net8.0-windows/win-x64/publish/
```

---

## 6.2 Add Publish to CI

Add to workflow:

```yaml
- name: Publish
  run: dotnet publish src/PotatoWpfCICD/PotatoWpfCICD.csproj -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true
```

---

# 📤 Step 7: Upload Artifact (Downloadable EXE)

## 7.1 Artifact Step

```yaml
- name: Upload Artifact
  uses: actions/upload-artifact@v4
  with:
    name: PotatoWpfCICD
    path: |
      PotatoWpfCICD/bin/Release/**/publish/
```

🎉 Now GitHub gives you:

* ZIP file
* Contains your WPF EXE
* Downloadable from **Actions → Artifacts**

---

# 🏁 FULL FINAL CI/CD FILE

```yaml
name: WPF CI/CD

on:
  push:
    branches: [ "main" ]

jobs:
  build:
    runs-on: windows-latest

    steps:
    - uses: actions/checkout@v4

    - uses: actions/setup-dotnet@v4
      with:
        dotnet-version: '8.0.x'

    - name: Restore
      run: dotnet restore

    - name: Build
      run: dotnet build -c Release

    - name: Test
      run: dotnet test -c Release

    - name: Publish
      run: dotnet publish src/PotatoWpfCICD/PotatoWpfCICD.csproj -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true

    - name: Upload Artifact
      uses: actions/upload-artifact@v4
      with:
        name: PotatoWpfCICD
        path: PotatoWpfCICD/bin/Release/**/publish/
```

---

# 🚀 Step 8: Advanced CD

## 🔹 Auto Versioning

```yaml
env:
  VERSION: 1.0.${{ github.run_number }}
```

## 🔹 Create GitHub Release

```yaml
- uses: softprops/action-gh-release@v2
  with:
    files: PotatoWpfCICD/bin/Release/**/publish/*
```

## 🔹 Code Signing (Real Company Stuff)

* Use `.pfx`
* Store cert in **GitHub Secrets**
* Sign with `signtool.exe`

---

# Recreate the solution file
1. Delete old solution
   `del PotatoWpfCICD.sln`
3. Create new solution
   `dotnet new sln -n PotatoWpfCICD`
5. Add Projects with correct paths
   `dotnet sln add src\PotatoWpfCICD\PotatoWpfCICD.csproj`
   `dotnet sln add tests\PotatoWpfCICD.Tests\PotatoWpfCICD.Tests.csproj`
7. dotnet restore
   ```
     dotnet restore
     dotnet build -c Release
     dotnet test -c Release
     dotnet publish src/PotatoWpfCICD/PotatoWpfCICD.csproj -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true
   ```
