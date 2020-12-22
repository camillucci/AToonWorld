import sys
from modules.analitics_reader import AnaliticsReader
from modules.analizers.deaths_analizer import DeathsAnalizer

analiticsFilePath = 'Analitics_production.csv'
if len(sys.argv) > 1 and sys.argv[1] == 'debug':
    analiticsFilePath = "Analitics_debug.csv"

linesToSkip = 0
if len(sys.argv) > 2:
    linesToSkip = sys.argv[2]

reader = AnaliticsReader(analiticsFilePath)
analitics = reader.ParseFile(int(linesToSkip))

# Start analize the analitics

deathAnalizer = DeathsAnalizer()
deathAnalizer.Analize(analitics)

print('mean deaths per level: ', deathAnalizer.meanLevelDeaths)
print('\nDeaths per player and level:')
for item in deathAnalizer.players:
    print(item)