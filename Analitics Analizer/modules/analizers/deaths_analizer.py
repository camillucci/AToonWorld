from ..analitic import Analitic

class DeathsAnalizer:
    players = []
    meanLevelDeaths = {}

    def Analize(self, analitics):
        for analitic in analitics:
            self.AddPlayerIfNotPresent(analitic)
            if str(analitic.eventName) == 'PlayerDeath':
                self.AddDeath(analitic)
        self.AnalizeMeanLevelDeaths()
            
    def AddPlayerIfNotPresent(self, analitic):
        present = False
        for player in self.players:
            if player['user'] == analitic.user:
                present = True
        if not present:
            self.players.append({ 'user': analitic.user })

    def AddDeath(self, analitic):
        for player in self.players:
            if player['user'] == analitic.user:
                if analitic.level in player:
                    player.update({ analitic.level: player[analitic.level] + 1})
                else:
                    player[analitic.level] = 1

    def AnalizeMeanLevelDeaths(self):
        indexes = {}
        for player in self.players:
            element = player.pop('user')
            for k, v in player.items():
                if k not in self.meanLevelDeaths:
                    self.meanLevelDeaths.update({ k: v })
                    indexes[k] = 0
                else:
                    self.meanLevelDeaths[k] = self.meanLevelDeaths[k] + v
                indexes[k] = indexes[k] + 1
        for k, v in self.meanLevelDeaths.items():
            self.meanLevelDeaths[k] = self.meanLevelDeaths[k] / indexes[k]
