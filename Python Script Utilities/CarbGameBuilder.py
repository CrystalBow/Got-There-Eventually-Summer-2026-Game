import json

# Global dictionary where data is stored.
FinalDict = {}
FinalDict["Ally"] = {}
FinalDict["Location"] = {}
FinalDict["Effects"] = {}
FinalDict["Cards"] = {}
FinalDict["Cards"]["Attacks"] = {}
FinalDict["Cards"]["Buffs"] = {}
FinalDict["Cards"]["Healing"] = {}
FinalDict["Cards"]["Debuffs"] = {}


def inputAndCheck(question, validAwnsers):
    """
    Asks a question to terminal and returns the integer value if it is valid. Will continue asking until a valid answer is received
    :param string question: A string to ask the user.
    :param list validAwnsers: A list of integers that are valid answers. if you want to accept any integer then put in an empty list.
    :return: An integer value if it is valid.
    """
    # Ask the question
    awnserString = input(question)
    # Prep the boolean and a place to store the integer.
    validaty = False
    convertedAwnser = 0
    awnserFlag = False
    # Using a try block for graceful handling of the conversion.
    try:
        # Verify it is an integer
        int(awnserString)
        validaty = True
    except ValueError:
        pass
    if validaty == False:
        # If it's not an integer ask again.
        return inputAndCheck(question, validAwnsers)
    else:
        convertedAwnser = int(awnserString)
        if len(validAwnsers) > 0:
            # If we have a list of valid answers we compare them with the given answer.
            for awnser in validAwnsers:
                if convertedAwnser == awnser:
                    awnserFlag = True
                else:
                    continue
        else:
            # If we don't have anything to compare with than it's automatically valid.
            awnserFlag = True
        if awnserFlag == False:
            # If it's not valid we ask again
            return inputAndCheck(question, validAwnsers)
        else:
            # Otherwise we return the valid awnser
            return convertedAwnser

def floatInput(question):
    """
    Asks a question to terminal and returns the float value. Will continue asking until a float value is received.
    :param question: The question to ask the user.
    :return: A Float value.
    """
    # Receive input
    awnserString = input(question)
    validaty = False
    # Verify it is a float
    try:
        float(awnserString)
        validaty = True
    except ValueError:
        pass
    if validaty == False:
        # Not a float? ask again!
        return floatInput(question)
    else:
        # Is a float? Return it!
        return float(awnserString)

def makeAlly():
    """
    Prompts for character information and stores it in the global FinalDict
    :return: Nothing
    """
    global FinalDict
    # Character Prompts: Name and base stats.
    name = input("Please enter a name: ")
    FinalDict["Ally"][name] = {}
    hp = inputAndCheck("What is their max HP?", [])
    mp = inputAndCheck("What is their max MP?", [])
    atk = inputAndCheck("What is their ATK?", [])
    defense = inputAndCheck("What is their DEF?", [])
    speed = inputAndCheck("What is their speed?", [])
    # Character Prompts: Scaling Ratios
    hpScale = floatInput("What is their HP Scaling?")
    mpScale = floatInput("What is their MP Scaling?")
    atkScale = floatInput("What is their ATK Scaling?")
    defenseScale = floatInput("What is their DEF Scaling?")
    speedScale = floatInput("What is their speed Scaling?")
    # Store in everything in dictionary
    FinalDict["Ally"][name]["hp"] = hp
    FinalDict["Ally"][name]["mp"] = mp
    FinalDict["Ally"][name]["atk"] = atk
    FinalDict["Ally"][name]["def"] = defense
    FinalDict["Ally"][name]["spd"] = speed
    FinalDict["Ally"][name]["hpScale"] = hpScale
    FinalDict["Ally"][name]["mpScale"] = mpScale
    FinalDict["Ally"][name]["atkScale"] = atkScale
    FinalDict["Ally"][name]["defScale"] = defenseScale
    FinalDict["Ally"][name]["speedScale"] = speedScale
    # Prep the specialty cards area
    FinalDict["Ally"][name]["Specialty Cards"] = {}
    FinalDict["Ally"][name]["Specialty Cards"]["Attacks"] = {}
    FinalDict["Ally"][name]["Specialty Cards"]["Buffs"] = {}
    FinalDict["Ally"][name]["Specialty Cards"]["Healing"] = {}
    FinalDict["Ally"][name]["Specialty Cards"]["Debuff"] = {}
    # Card loop for adding 3 cards. If you use the same name it'll over right the previous one you put in and still count it.
    cardCounter = 0
    while cardCounter < 3:
        TypeAnswer = inputAndCheck("What type of card are you adding?(0: Attacks,1: Buffs,2: Healing,3: Debuffs) ",
                                   [0, 1, 2, 3])
        card = constructCard()
        if TypeAnswer == 0:
            FinalDict["Ally"][name]["Specialty Cards"]["Attacks"].update(card)
        elif TypeAnswer == 1:
            FinalDict["Ally"][name]["Specialty Cards"]["Buffs"].update(card)
        elif TypeAnswer == 2:
            FinalDict["Ally"][name]["Specialty Cards"]["Healing"].update(card)
        elif TypeAnswer == 3:
            FinalDict["Ally"][name]["Specialty Cards"]["Debuff"].update(card)
        cardCounter += 1





def makeLocation():
    """
    Prompts for and makes a new location for enemy specifications
    :return: Nothing
    """
    global FinalDict
    name = input("Please enter a name: ")
    FinalDict["Location"][name] = {}

def makeEnemy():
    """
    Prompts for and stores enemy data
    :return: Nothing
    """
    global FinalDict
    # Prompt and ask for the location of this enemy
    for location in FinalDict["Location"]:
        print(location)
    locationName= input("Please enter a location: ")
    if locationName in FinalDict["Location"]:
        # Starts collecting and storing data if the location exists
        print("Location Found! Ready to populate!")
        # Name and stat prompts
        name = input("Please enter a name: ")
        FinalDict["Location"][locationName][name] = {}
        hp = inputAndCheck("What is their HP?", [])
        atk = inputAndCheck("What is their ATK?", [])
        defense = inputAndCheck("What is their DEF?", [])
        speed = inputAndCheck("What is their speed?", [])
        # Store Data
        FinalDict["Location"][locationName][name]["hp"] = hp
        FinalDict["Location"][locationName][name]["atk"] = atk
        FinalDict["Location"][locationName][name]["def"] = defense
        FinalDict["Location"][locationName][name]["speed"] = speed
    else:
        # Does nothing if the location has not been made.
        print("Location Not Found! Please make the location first!")
        return

def constructCard():
    """
    Prompts for and stores card data
    :return: the card data in dictionary format
    """
    name = input("Please enter a name: ")
    description = input("Please enter a description: ")
    damage = inputAndCheck("How much damage? (if it's healing enter a negative number)", [])
    time = inputAndCheck("How long does it last? (if it's instant just put a -1)", [])
    cost = inputAndCheck("How much cost? (if it restores then enter a negative number)", [])
    effects = effectPrompt()
    return {name: {"description": description, "damage": damage, "time": time, "cost": cost, "effects": effects}}

def creationIteration():
    """
    The iteration for main
    :return: Boolean for the loop
    """
    stopFlag = False
    answer = inputAndCheck("What would like to make? (0:Ally, 1:Location, 2:Enemy, 3:Card, 4:Effect, 5:Finish)", [0, 1, 2, 3, 4, 5])
    if answer == 0:
        makeAlly()
    elif answer == 1:
        makeLocation()
    elif answer == 2:
        makeEnemy()
    elif answer == 3:
        genricCardMaker()
    elif answer == 4:
        effectAdd()
    elif answer == 5:
        stopFlag = True
    return stopFlag



def genricCardMaker():
    """
    Asks what category your storing the card in and puts it all in global FinalDict.
    :return: Nothing
    """
    global FinalDict
    TypeAnswer = inputAndCheck("What type of card are you adding?(0: Attacks,1: Buffs,2: Healing,3: Debuffs) ", [0,1,2,3])
    card = constructCard()
    if TypeAnswer == 0:
        FinalDict["Cards"]["Attacks"].update(card)
    elif TypeAnswer == 1:
        FinalDict["Cards"]["Buffs"].update(card)
    elif TypeAnswer == 2:
        FinalDict["Cards"]["Healing"].update(card)
    elif TypeAnswer == 3:
        FinalDict["Cards"]["Debuffs"].update(card)

def effectPrompt():
    global FinalDict
    answer = inputAndCheck("Would you like to add an effect?(0:No, 1:Yes)", [0, 1])
    collectEffects = []
    while answer != 0:
        validIDS = [0]
        curr = 1
        for effect in FinalDict["Effects"]:
            print(str(curr) +": " + effect)
            validIDS.append(curr)
            curr += 1
        answer = inputAndCheck("Please enter valid effect ID!", validIDS)
        collectEffects.append(answer)
        answer = inputAndCheck("Would you like to add an effect?(0:No, 1:Yes)", [0, 1])
    return collectEffects
def effectAdd():
    """
    prompts for and adds an effect with a unique ID to global FinalDict.
    :return: Nothing
    """
    global FinalDict
    global effectIds
    name = input("Please enter a name: ")
    FinalDict["Effects"][name] = effectIds
    effectIds += 1



stopFlag = False
selectFlag = False
# effect 0 is no effect.
effectIds = 1
mode = inputAndCheck("Are you adding to or making newGamedata (0: add, 1: new, 2 exit)", [0,1,2])
while not stopFlag:
    if mode == 0:
        jFile = open("GameData.json", "r")
        FinalDict = json.load(jFile)
        print(FinalDict)
        jFile.close()
        mode = 1
        effectIds = len(FinalDict["Effects"]) + 1
    elif mode == 1:
        stopFlag = creationIteration()
    elif mode == 2:
        stopFlag = True

print("Prepping JSON")
jFile = open("GameData.json", "w").write(json.dumps(FinalDict, indent=4))
