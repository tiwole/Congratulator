# Guess Figma Screenshot

Take a screenshot of the currently selected element in Figma via MCP and provide a brief description of what is shown, along with a conclusion about what it might be (component, page, UI element, etc.).

## Instructions

1. Use the `mcp__localFigma__get_screenshot` tool without the nodeId parameter (to get the screenshot of the selected element)
2. Analyze the received image
3. Provide a brief description (2-3 sentences) of what is shown
4. Make a conclusion about what it might be:
   - Component type (button, form, card, table, modal, etc.)
   - Element purpose
   - Design features

MCP tool parameters:
- clientLanguages: "csharp,html,css"
- clientFrameworks: "blazor,bootstrap"
