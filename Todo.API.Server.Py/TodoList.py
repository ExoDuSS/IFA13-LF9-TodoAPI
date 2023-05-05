import uuid

class TodoList:
    def __init__(self, name):
        self.Id = uuid.uuid4()
        self.Name = name
        self.Entries = []
    def __init__(self, id, name):
        self.Id = id
        self.Name = name
        self.Entries = []
