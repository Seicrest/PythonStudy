from search import *
from nn_puzzle import *

class BFS(Search):

    def search(self, initial_state):
        """Given an initial problem state, encode a search-tree
        node representing the state and systematically explore
        the state-space until a goal-state is found.

        Returns:
         a search-tree node representing a goal state (if found)
         or None, if no goal is discovered"""
        #dictionary key state value parent state current depth and action
        self.VisitedState={}
        #first node parent is None depth is 0
        self.VisitedState[initial_state]=(None,0,'Start')
        level=0
        nextNodes=self.getNodeByDepth(level)
        #TODO:use queue
        while len(nextNodes)>0:
            for node in nextNodes:
                if self.problem.isgoal(node):
                    return node
                else:
                    nextActions = self.problem.actions(node)
                    for action in nextActions:
                        newState=self.child_node(node,action)
                        if newState not in self.VisitedState.keys():
                            #skip when already visit
                            self.VisitedState[newState]=(node,level+1,action)
                    
            #try to get next level
            level+=1
            nextNodes=self.getNodeByDepth(level)
        return None

    #return a list of Node with depth = level
    def getNodeByDepth(self, level):
        result=[]
        for i in self.VisitedState:
            if self.VisitedState[i][1]==level:
                result.append(i)
        return result
    
    def solution(self, node):
        """Returns the 'solution' for the specified node in the search tree.
        That is, this method should return a sequence of tuples:
        [(state_0, action_0), (state_1, action_1), ..., (state_n, action_n)]

        such that:
        state_0 is the initial state in the search
        action_0 is the first action taken
        action_i is the action taken to transition from state_i to state_{i+1}
        state_n is the state encapsulated by the 'search_node' argument
        action_n is None
        """
        path=[]
        while node!= None and self.VisitedState[node][1]!=0:
            path.insert(0,(node,self.VisitedState[node][2]))
            node=self.parent(node)
        return path    

    def child_node(self, node, action):
        """Create a child node for this search tree given a parent
        search tree node and an action to execute. Don't confuse nodes
        in the search-tree and verticies in the state-space graph."""
        return self.problem.result(node, action)[0]

    def parent(self, node):
        """Return the parent of the specified node in the search tree"""
        if node in self.VisitedState.keys():
            return self.VisitedState[node][0]
        return None

    def depth(self, node):
        """Determine how deep the search tree node is in the search tree.
        Consider the initial state (root) to be at depth 0

        Note: this method is NOT required by the Search class interface. But
        if you wanted to implement BFS or DFS or a variant, you'd likely
        want such a function.
        """
        if node in self.VisitedState.keys():
            return self.VisitedState[node][1]
        return -1

if __name__ == "__main__":

    puzzle = NNPuzzle(3)
    initial = puzzle.get_shuffled_state(20)
    solver = BFS(puzzle)
    goal = solver.search(initial)
    print("Initial State", initial)
    puzzle.display((initial,'Start'))
    print("Found goal in "+str(solver.depth(goal)))
    for sa in solver.solution(goal):
        puzzle.display(sa)
