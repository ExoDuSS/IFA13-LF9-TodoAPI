import json

from TodoEntry import TodoEntry
from TodoList import TodoList

class MyJsonEncoder(json.JSONEncoder):
    def default(self, obj):
        if isinstance(obj, TodoEntry):
            return {"Id": obj.Id, "ListId": obj.ListId ,"Name": obj.Name , "Description": obj.Description}
        elif isinstance(obj, TodoList):
            return {"Id": obj.Id, "Name": obj.Name, "Entries": obj.Entries}
        #elif isinstance(obj, list):
        #    return [self.default(item) for item in obj]
        try:
            return json.JSONEncoder.default(self, obj)
        except TypeError:
            return str(obj)
    
class MyJsonDecoder(json.JSONDecoder):
    def __init__(self, *args, **kwargs):
        super().__init__(object_hook=self.object_hook, *args, **kwargs)

    def object_hook(self, obj_dict):
        if 'Id' in obj_dict and 'ListId' in obj_dict and 'Name' in obj_dict and 'Description' in obj_dict:
            entry = TodoEntry(obj_dict['ListId'], obj_dict['Name'], obj_dict['Description'])
            entry.Id = obj_dict['Id']
            return entry
        elif 'Id' in obj_dict and 'Name' in obj_dict and 'Entries' in obj_dict:
            list = TodoList(obj_dict['Id'], obj_dict['Name'])
            list.Entries = obj_dict['Entries']
            return list
        return obj_dict