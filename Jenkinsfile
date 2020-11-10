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
        echo 'Test Publish'
      }
    }

  }
  environment {
    UNITY_EDITOR = '/opt/unity_editor/Editor/Unity'
  }
}