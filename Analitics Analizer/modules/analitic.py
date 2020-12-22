from enum import Enum

class EventName(Enum):
    PlayerDeath = 1
    LevelTime = 2                       # Seconds spent on a level
    CheckpointTime = 3                  # Seconds spent to reach a checkpoint from the previous one - [ 2, 3, 26 ]
    InkFinished = 4                     # A specific ink is finished
    InkStatusAtCheckpoint = 5           # Inks status (amount) when the player reach a checkpoint
    LevelFeedback = 6                   # Scene and user feedback
    EnemyKilled = 7     

class Analitic:
    values = []

    def __init__(self, user, level, game, datetime, eventName):
        self.user = user
        self.level = level
        self.datetime = datetime
        self.eventName = eventName
        self.game = game

    def ParseFromCsv(line):
        line = line[1:-1]
        items = line.split('","')
        analitic = Analitic (
            items[1],
            items[2],
            items[3],
            items[4],
            items[5]
        )

        if len(items) > 5:
            analitic.values = items[5:]

        return analitic