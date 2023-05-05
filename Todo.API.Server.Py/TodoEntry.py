import uuid

class TodoEntry:
    def __init__(self, name, description):
        self.Id = uuid.uuid4()
        self.ListId = None
        self.Name = name
        self.Description = description
    
    def __init__(self, list_id, name, description):
        self.Id = uuid.uuid4()
        self.ListId = list_id
        self.Name = name
        self.Description = description