pipeline {
  agent any
  stages {
    stage('Build') {
      steps {
        sh '$UNITY_EDITOR -batchmode -nographics -projectPath "./A Toon World" -buildTarget WebGL -customBuildTarget WebGL -customBuildName WebGLBuild -customBuildPath Build -quit'
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