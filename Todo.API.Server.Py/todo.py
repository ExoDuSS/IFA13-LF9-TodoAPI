import uuid

class todo:
    def __init__(self, name, description):
        self.uuid = uuid.uuid4
        self.uuid_list = None
        self.name = name
        self.description = description
    
    def __init__(self, list_id, name, description):
        self = self.__init__(name,description)
        self.uuid_list = list_id
    
    def setListUUID(self, id):
        self.uuid_list = id