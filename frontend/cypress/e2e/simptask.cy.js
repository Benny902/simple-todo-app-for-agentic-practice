describe('Simple Task Manager E2E', () => {
  beforeEach(() => {
    // Visit the app
    cy.visit('http://localhost:5173')
  })

  it('US1: Can add and view a task', () => {
    const taskTitle = 'E2E Test Task ' + Date.now()
    
    // Check initial state or loading
    // Type in input
    cy.get('input[placeholder="What needs to be done?"]').type(taskTitle)
    
    // Click Add
    cy.contains('button', 'Add Task').click()

    // Verify task appears in list
    cy.contains('.task-title', taskTitle).should('be.visible')
  })

  it('US2: Can complete a task', () => {
    const taskTitle = 'Task to Complete ' + Date.now()
    
    // Add task first
    cy.get('input[placeholder="What needs to be done?"]').type(taskTitle)
    cy.contains('button', 'Add Task').click()

    // Find the task item containing the title
    cy.contains('.task-item', taskTitle).within(() => {
      // Check the checkbox
      cy.get('input[type="checkbox"]').check()
      
      // Verify completed style (optional, but good)
      // We check if the task item has 'completed' class based on our component logic
      // Note: React might take a moment to update class, cy.get retries automatically
    })
    
    // Verify the parent li has completed class
    cy.contains('.task-item', taskTitle).should('have.class', 'completed')
  })

  it('US3: Can filter tasks by status', () => {
    const activeTask = 'Active Task ' + Date.now()
    const completedTask = 'Completed Task ' + Date.now()

    // Add two tasks
    cy.get('input[placeholder="What needs to be done?"]').type(activeTask)
    cy.contains('button', 'Add Task').click()
    
    cy.get('input[placeholder="What needs to be done?"]').type(completedTask)
    cy.contains('button', 'Add Task').click()

    // Complete the second task
    cy.contains('.task-item', completedTask).within(() => {
      cy.get('input[type="checkbox"]').check()
    })

    // Test "All" filter (default)
    cy.get('.filter-select').should('have.value', 'all')
    cy.contains('.task-title', activeTask).should('be.visible')
    cy.contains('.task-title', completedTask).should('be.visible')

    // Test "Active" filter
    cy.get('.filter-select').select('active')
    cy.contains('.task-title', activeTask).should('be.visible')
    cy.contains('.task-title', completedTask).should('not.exist')

    // Test "Completed" filter
    cy.get('.filter-select').select('completed')
    cy.contains('.task-title', activeTask).should('not.exist')
    cy.contains('.task-title', completedTask).should('be.visible')

    // Back to "All"
    cy.get('.filter-select').select('all')
    cy.contains('.task-title', activeTask).should('be.visible')
    cy.contains('.task-title', completedTask).should('be.visible')
  })

  it('US4: Can sort tasks by title', () => {
    const taskA = 'A Task ' + Date.now()
    const taskZ = 'Z Task ' + Date.now()
    const taskM = 'M Task ' + Date.now()

    // Add tasks in random order
    cy.get('input[placeholder="What needs to be done?"]').type(taskZ)
    cy.contains('button', 'Add Task').click()
    
    cy.get('input[placeholder="What needs to be done?"]').type(taskA)
    cy.contains('button', 'Add Task').click()
    
    cy.get('input[placeholder="What needs to be done?"]').type(taskM)
    cy.contains('button', 'Add Task').click()

    // Sort by title A-Z
    cy.get('.sort-select').select('title-asc')
    cy.get('.task-title').eq(0).should('contain', taskA)
    cy.get('.task-title').eq(1).should('contain', taskM)
    cy.get('.task-title').eq(2).should('contain', taskZ)

    // Sort by title Z-A
    cy.get('.sort-select').select('title-desc')
    cy.get('.task-title').eq(0).should('contain', taskZ)
    cy.get('.task-title').eq(1).should('contain', taskM)
    cy.get('.task-title').eq(2).should('contain', taskA)
  })

  it('US5: Can sort tasks by date', () => {
    const task1 = 'First Task ' + Date.now()
    const task2 = 'Second Task ' + Date.now()
    const task3 = 'Third Task ' + Date.now()

    // Add tasks sequentially
    cy.get('input[placeholder="What needs to be done?"]').type(task1)
    cy.contains('button', 'Add Task').click()
    cy.wait(100) // Small delay to ensure different timestamps
    
    cy.get('input[placeholder="What needs to be done?"]').type(task2)
    cy.contains('button', 'Add Task').click()
    cy.wait(100)
    
    cy.get('input[placeholder="What needs to be done?"]').type(task3)
    cy.contains('button', 'Add Task').click()

    // Default should be newest first
    cy.get('.sort-select').should('have.value', 'date-desc')
    cy.get('.task-title').eq(0).should('contain', task3)
    cy.get('.task-title').eq(2).should('contain', task1)

    // Sort by date oldest first
    cy.get('.sort-select').select('date-asc')
    cy.get('.task-title').eq(0).should('contain', task1)
    cy.get('.task-title').eq(2).should('contain', task3)
  })
})

