import sys

# Print iterations progress
def printProgressBar (iteration, total, prefix = '', suffix = '', decimals = 1, length = 100, fill = '█', printEnd = "\r"):
    """
    Call in a loop to create terminal progress bar
    @params:
        iteration   - Required  : current iteration (Int)
        total       - Required  : total iterations (Int)
        prefix      - Optional  : prefix string (Str)
        suffix      - Optional  : suffix string (Str)
        decimals    - Optional  : positive number of decimals in percent complete (Int)
        length      - Optional  : character length of bar (Int)
        fill        - Optional  : bar fill character (Str)
        printEnd    - Optional  : end character (e.g. "\r", "\r\n") (Str)
    """
    percent = ("{0:." + str(decimals) + "f}").format(100 * (iteration / float(total)))
    filledLength = int(length * iteration // total)
    bar = fill * filledLength + '-' * (length - filledLength)
    print(f'\r{prefix} |{bar}| {percent}% {suffix}', end = printEnd)
    # Print New Line on Complete
    if iteration == total: 
        print()

def ReplaceFirstWithIndex(line):
    line = line[1:]
    commaIndex = line.index(',')
    return line[commaIndex:]

def InsertBefore(text, line):
    return f"\"{text}\"{line}"

# START #

if len(sys.argv) < 2:
    print('Specific a csv file as parameter')
    exit()

filename = sys.argv[1]
file = open(filename)

total = sum(1 for line in file)
file.seek(0, 0)

lines = []

i = 0
for line in file:
    withoutFirst = ReplaceFirstWithIndex(line)
    if i == 0:
        withId = InsertBefore("id", withoutFirst)
    else:
        withId = InsertBefore(str(i), withoutFirst)
    lines.append(withId)
    i = i + 1
    printProgressBar(i, total, f'Parsing {filename}', f'Complete [Total: {total}]')

file.close()

outfilename = 'formatted.csv'
print('saving to', outfilename)

outfile = open(outfilename, 'w')
outfile.writelines(lines)
outfile.close()

print('Complete')