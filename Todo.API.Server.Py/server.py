import uuid
import JSONUtils

from TodoEntry import TodoEntry
from TodoList import TodoList
from flask import Flask, request, jsonify, abort
from copy import copy

app = Flask(__name__)

TodoLists = {}
Todos = {}

# add some headers to allow cross origin access to the API on this server, necessary for using preview in Swagger Editor!
@app.after_request
def apply_cors_header(response):
    response.headers['Access-Control-Allow-Origin'] = '*'
    response.headers['Access-Control-Allow-Methods'] = 'GET,POST,DELETE'
    response.headers['Access-Control-Allow-Headers'] = 'Content-Type'
    return response

# 
# POST new list
# GET all lists
@app.route('/todo-list/', methods=['POST', 'GET'])
def handle_base_list():
    if request.method == 'POST':
        new_list = request.get_json(force=True)
        id = uuid.uuid4();
        TodoLists[id] = TodoList(id,new_list['name'])
        return jsonify(TodoLists[id]), 200
    elif request.method == 'GET':
        return jsonify(buildAllCompleteTodoList()), 200

# define endpoint for getting and deleting existing todo lists
# GET get all entries
# DELETE remove list with all entries
@app.route('/todo-list/<listId>', methods=['GET', 'DELETE'])
def handle_list(listId):
    # find todo list depending on given list id
    list_item = getList(listId)
    # if the given list id is invalid, return status code 404
    if not list_item:
        abort(404)
    if request.method == 'GET':
        # find all todo entries for the todo list with the given id
        print('Returning todo list...')
        return jsonify(buildCompleteTodoList())
        #return jsonify([i for i in Todos if i['listId'] == listId])
    elif request.method == 'DELETE':
        # delete list with given id
        print('Deleting todo list...')
        removeTodoList(listId)
        return '', 200

#
# POST add entry to list
@app.route('/todo-list/<listId>/entry', methods=['POST'])
def handle_base_entry(listId):
    # find todo list depending on given list id
    list_item = None
    for l in TodoLists:
        if l['id'] == listId:
            list_item = l
            break
    # if the given list id is invalid, return status code 404
    if not list_item:
        abort(404)
    new_entry = request.get_json(force=True)
    new_entry['id'] = uuid.uuid4
    new_entry['listId'] = listId
    Todos.append(new_entry)
    return jsonify(new_entry), 200

#
# GET return an entry
# PUT update an entry
# DELETE remove an entry
@app.route('/todo-list/<listId>/entry/<entryId>', methods=['GET','PUT','DELETE'])
def handle_entry(listId, entryId):
    # find todo list depending on given list id
    list_item = getList(listId)
    # if the given list id is invalid, return status code 404
    if not list_item:
        abort(404)

    entry_item = getEntry(entryId)
    if not entry_item:
        abort(404)
    
    if request.method == 'GET':
        return jsonify(entry_item), 200
    elif request.method == 'PUT':
        update_entry = request.get_json(force=True)

        Todos[entryId].Name = update_entry['name']
        Todos[entryId].Description = update_entry['description']

        return jsonify(Todos[entryId]), 200
    elif request.method == 'DELETE':
        Todos.remove(entryId)

# helper methods
def buildAllCompleteTodoList():
    complete_lists = []
    for key in TodoLists:
        complete_lists.append(buildCompleteTodoList(key))
    return complete_lists

def buildCompleteTodoList(list_id):
    list = getList(list_id)

    if not list:
        return None

    list.Entries = []
    for key in Todos:
        if Todos[key].ListId == list_id:
            list.Entries.append(Todos[key])
    return list

def removeTodoList(list_id):
    if listExists(list_id):
        del TodoLists[list_id]
        for key in Todos:
            if Todos[key].ListId == list_id:
                del Todos[key]

def getList(list_id):
    if listExists(list_id):
        return copy(TodoLists[list_id])
    else:
        return None

def getEntry(entry_id):
    if entryExists(entry_id):
        return Todos[entry_id]
    else:
        return None

def listExists(list_id):
    for k in TodoLists:
        if k == list_id:
            return True
    return False

def entryExists(entry_id):
    for k in Todos:
        if k == entry_id:
            return True
    return False;

def addMockData():
    # create unique id for lists, entries
    todo_list_1_id = '1318d3d1-d979-47e1-a225-dab1751dbe75'
    todo_list_2_id = '3062dc25-6b80-4315-bb1d-a7c86b014c65'
    todo_list_3_id = '44b02e00-03bc-451d-8d01-0c67ea866fee'
    todo_1_id = uuid.uuid4()
    todo_2_id = uuid.uuid4()
    todo_3_id = uuid.uuid4()
    todo_4_id = uuid.uuid4()

    # define internal data structures with example data
    global TodoLists
    TodoLists = {
        todo_list_1_id: TodoList(todo_list_1_id, 'Einkaufsliste'),
        todo_list_2_id: TodoList(todo_list_1_id, 'Arbeit'),
        todo_list_3_id: TodoList(todo_list_1_id, 'Privat')
    }
    global Todos
    Todos = {
        todo_1_id: TodoEntry(todo_list_1_id, 'Milch', '3x'),
        todo_2_id: TodoEntry(todo_list_1_id, 'Eier', '6x'),
        todo_3_id: TodoEntry(todo_list_2_id, 'Snackbox auffüllen', 'Mit geilem Scheiß'),
        todo_4_id: TodoEntry(todo_list_3_id, 'Sauber machen', 'Jetzt aber wirklich!')
    }

if __name__ == '__main__':
    addMockData()
    app.debug = True
    app.json_encoder = JSONUtils.MyJsonEncoder
    app.json_decoder = JSONUtils.MyJsonDecoder
    app.run(host='0.0.0.0', port=8080)