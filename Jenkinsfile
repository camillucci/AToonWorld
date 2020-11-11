pipeline {
  agent any
  stages {
    stage('Build') {
      steps {
        sh '$UNITY_EDITOR -batchmode -executeMethod BuildCommand.PerformBuild -nographics -projectPath "./A Toon World" -buildTarget WebGL -quit'
      }
    }

    stage('Publish') {
      steps {
        sshPublisher(
              continueOnError: false, 
              failOnError: true,
              publishers: [
                sshPublisherDesc(
                  configName: "webmachine_web_inkverse",
                  transfers: [sshTransfer(sourceFiles: './A Toon World/Build/**/*',
                                          removePrefix: 'A Toon World/Build',
                                          remoteDirectory: '${BRANCH_NAME}',
                                          cleanRemote: true,
                                          makeEmptyDirs: true)],
                  verbose: true
                )
              ]
            )
      }
    }

  }
  environment {
    UNITY_EDITOR = '/opt/unity_editor/Editor/Unity'
  }
}