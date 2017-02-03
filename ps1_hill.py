from os import listdir
from os.path import isfile, join
import matplotlib.pyplot as plt
mypath="Liens-50"

#1.dictonary mapping the file name (without the directoryname) to a string representing the entire text document.
def create_corpus(directory):
    dic={}
    for f in listdir(directory):
        if isfile(join(directory, f)):
            #file name in same directory is different not duplicate key
            dic[f]=open(join(directory, f),'r').read()
    return dic
files=create_corpus(mypath)

#2.return the shortest file name and longest file name
#[{'Length': 946, 'Name': 'WA_Kitsap_2009-08-25__200908250022.txt'}
#{'Length': 9647, 'Name': 'OR_Coos_2008-04-03__08003299.txt'}]

def corpus_char_stats(corpus):
    result=[]
    if not corpus:
        print "Dict is empty"
    else:
        for key in corpus:
            length=len(corpus[key])
            if len(result)==0:                
                result.append({"Name":key,"Length":length})
                result.append({"Name":key,"Length":length})
            else:
                if(length<result[0]["Length"]):
                    result[0]["Name"]=key
                    result[0]["Length"]=length
                elif (length>result[1]["Length"]):
                    result[1]["Name"]=key
                    result[1]["Length"]=length
    return result

print(corpus_char_stats(files))

#3 splits the data string into a sequence of white-space delimited tokens
def isAlphabetic(str):
    for i in str:
        if((i>='a'and i<='z') or (i>='A'and i<='Z')):
           continue
        else:
            return False
    return True

def words(data):
    alphabetic=[]       #alphabetaic words lower case
    non_alphabetic=[]   #non alphabetaic words
    words=data.split(' ')
    for word in words:
           if len(word)>0:
               if isAlphabetic(word):
                   alphabetic.append(word.lower())
               else:
                   non_alphabetic.append(word)
    return {"Alphabetic":alphabetic,"Non_alphabetic":non_alphabetic}

print(words(files[corpus_char_stats(files)[0]["Name"]])) 

#4 word ratios for each file
#{'Ratio': 0.43342036553524804, 'Name': 'WA_Benton_2009-04-06__2009-009261.txt'}
def find_word_ratios(corpus):
    result=[]
    if not corpus:
        print "Dict is empty"
    else:
        for key in corpus:
            splitWords=words(corpus[key])
            ratio=len(splitWords["Alphabetic"])*1.0/(len(splitWords["Alphabetic"])+len(splitWords["Non_alphabetic"]))
            result.append({"Name":key,"Ratio":ratio})
    result.sort(key=lambda item:item["Ratio"])
    return result

print(find_word_ratios(files))

#5 'of', 'the', 'and', 'to', 'or', 'lien', 'a', 'in', 'for', 'is'
def word_frequencies(corpus):
    result={}
    if not corpus:
        print "Dict is empty"
    else:
        for key in corpus:
            splitWords=words(corpus[key])
            alphWords=splitWords["Alphabetic"]
            for w in alphWords:
                if w in result.keys():
                    result[w]+=1
                else:
                    result[w]=1
    resultList=[]
    for key, value in result.iteritems():
        resultList.append({"Frequenct":value,"Word":key})
    #result=sorted(result, key=result.get, reverse=True)
    resultList.sort(key=lambda item:item["Frequenct"], reverse=True)
    return resultList
wordsFre=word_frequencies(files)
print(wordsFre[0])
#6 draw chart

plt.figure(1)
plt.plot([x['Frequenct'] for x in wordsFre])
#plt.axis([0, 1400, 0, 800])
plt.xlabel("Rank")
plt.ylabel("Frequenct")
plt.title("Word Frequenct")
plt.savefig("loglog.pdf") # Could also have written svg, png, ...
plt.show() # Show figure in window
