pipeline {
  agent { label 'unity' }
  stages {
    stage('Build') {
      steps {
        sh '$UNITY_EDITOR -batchmode -executeMethod BuildCommand.PerformBuild -nographics -projectPath "./A Toon World" -buildTarget WebGL -quit'
      }
    }

    stage('Publish') {
      when {
        not {
          anyOf {
            branch 'PR-*';
            branch 'pr-*'
          }
        }
      }
      steps {
        sshPublisher(
              continueOnError: false, 
              failOnError: true,
              publishers: [
                sshPublisherDesc(
                  configName: "webmachine_web_inkverse",
                  transfers: [sshTransfer(sourceFiles: 'Build/**/*',
                                          removePrefix: 'Build',
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