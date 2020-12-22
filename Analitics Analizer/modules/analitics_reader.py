from .analitic import Analitic
from .utils import *

class AnaliticsReader:
    path = ''

    def __init__(self, path):
        self.path = path

    def ParseFile(self, linesToSkip):
        file = self.OpenFile()
        analitics = []
        total = self.FileLinesCount(file) - linesToSkip
        lineNum = 0
        for line in file:
            lineNum = lineNum + 1
            if line != '' and lineNum > linesToSkip:
                analitics.append(Analitic.ParseFromCsv(line))
            printProgressBar(len(analitics), total, f'Parsing {self.path}', f'Complete [Total: {total}]')
        file.close()
        return analitics

    def OpenFile(self):
        try:
            file = open(self.path)
            return file
        except:
            exit("ciao")

    def FileLinesCount(self, file):
        total = sum(1 for line in file)
        file.seek(0, 0)
        return total