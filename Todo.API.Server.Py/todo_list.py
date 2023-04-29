import uuid
import todo

class todo_list:
    def __init__(self, name):
        self.uuid = uuid.uuid4
        self.name = name
        self.entries = dict()

    def insertEntry(self, entry: todo):
        self.entries[entry.uuid] = entry
    
    def getEntry(self, id):
        return self.entries[id]