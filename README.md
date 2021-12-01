# DMRecorder : 디모이 녹음기

DMRecorder는 Windows App SDK 및 WinUI 3을 학습하기 위해 만든 간단한 녹음기 애플리케이션입니다. DMRecorder를 통해 다음의 Windows App SDK 및 WinUI 3의 기능을 확인할 수 있습니다.


## 단일 패키지

원래 WinUI 3 프로젝트는 단일 패키지를 지원하지 않았습니다. 기본 프로젝트 구성은 애플리케이션 프로젝트와 패키지 프로젝트로 놔눠져 있었습니다. 이제 Windows App SDK의 단일 패키지 기능으로 응용 애플리케이션이면서 배포할 수 있는 단일 프로젝트로 구성할 수 있습니다.

![단일패키지](https://docs.microsoft.com/ko-kr/windows/apps/windows-app-sdk/images/single-project-overview.png)

단일 패키지는 이제 프로젝트 템플릿을 통해 쉽게 구성할 수 있습니다. `확장 관리`를 통해 `Single-project MSIX Packaging Tool for VS 2022`를 설치해야 합니다.


## 패키지, 비패키지 실행

Windows App SDK는 이제 `패키지` 실행과 `비패키지` 실행 모두 지원합니다. 패키지는 기존 MSIX으로 패키징 해서 설치하는 방식인데 Microsoft Store에 배포할 때 사용할 수 있습니다. 또한 비패키징 방식도 지원하는데 기존 설치툴을 그대로 이용하거나 개발시 `배포 시간`이 단축되므로 좀 더 빠르게 개발을 진행할 수 있습니다.

`패키지`와 `비패키지`은 프로파일을 선택해서 실행할 수 있는데 프로파일 설정은 다음과 같습니다.

| Properties/launchSettings.json
```json
{
  "profiles": {
    "Package": {
      "commandName": "MsixPackage"
    },
    "Unpackaged": {
      "commandName": "Project"
    }
  }
}
```

화면 상단의 다음의 영역으로 프로파일을 선택하고 실행할 수 있습니다.

![프로파일 선택](images/profile1.png)