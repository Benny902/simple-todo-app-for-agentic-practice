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
})
