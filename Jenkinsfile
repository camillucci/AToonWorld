pipeline {
  agent any
  stages {
    stage('Build') {
      steps {
        sh '$UNITY_EDITOR -batchmode -nographics -projectPath "$pwd/A Toon World" -buildTarget WebGL -quit'
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