## Game Overview
Your game revolves around managing a small Greek settlement, focusing on building up the economy,
producing essential goods, and expanding the city.

### Core Gameplay Mechanics
1. **Resource Management**\
   Players need to gather and manage three main categories of resources:
    - **Food**: Barley Bread and Wine are essential for keeping citizens fed and happy.
    - **Materials**: Wood, Stone, and Marble are necessary for constructing buildings and advancing the city.
    - **Tools**: Bronze Tools are used for improving production efficiency and enabling more advanced construction.
2. Production Chains\
   The game is built around simple **production chains** where players gather base resources and convert them into more advanced products.
    - **Barley Bread** is produced by growing **barley** at the **Farm** and milling it into flour at the **Mill**.
    - **Wine** is made from **grapes** at the **Vineyard**, which are then processed in the **Wine Press**.
    - **Tools** like **Bronze Tools** are crafted at the **Smith**, using resources from the **Bronze Mine**.
3. Building and Expansion\
   As players progress, they will need to build new **structures** like farms,
   mills, vineyards, and quarries. These buildings are essential for gathering
   resources and advancing the economy.

## Resources
We want to keep the resource system simple and avoid implementing too many resources.
For each category (Food, Money, Materials), we aim for a maximum of three different resources.
Each category should have a base material, easy to obtain for early development,
and an advanced material, produced by combining or refining the base material,
for more advanced buildings.

1. Food
    - **Barley Bread**: Farm → Mill → Flour
    - **Grapes/Wine**: Vineyard → Wine Press → Wine
2. Materials
    - **Silver Coins**: Taxes
    - **Wood**: Wood Cutter → Wood
    - **Stone**: Stone Quarry → Stone
    - **Marble**: Marble Quarry → Marble
3. Tools
    - **Bronze Tools**: Bronze Mine → Smith → Bronze Tools

---

## Buildings
These buildings form the core of the game’s progression. Players need to strategically balance resource production and
manage population size to advance through the game.

### **Farm**
- **Purpose**: The **Farm** is the foundation of your food production. It is where you grow **barley**, the primary ingredient for **Barley Bread**.
- **Function**:
    - **Produces Barley** over time.
    - **Requires Workers** to operate.

### **Mill**
- **Purpose**: The **Mill** grinds **barley** into **barley flour**, which is essential for baking **Barley Bread**.
- **Function**:
    - **Converts Barley** into **Barley Flour**.
    - **Requires Workers** to operate.

### **Vineyard**
- **Purpose**: The **Vineyard** is where **grapes** are grown, the first step in producing **wine**.
- **Function**:
    - **Produces Grapes** over time.
    - **Requires Workers** to harvest.

### **Wine Press**
- **Purpose**: The **Wine Press** processes **grapes** into **wine**, a luxury good.
- **Function**:
    - **Converts Grapes** into **Wine**.
    - **Requires Workers** to harvest.

### **Wood Cutter**
- **Purpose**: The **Wood Cutter** gathers **wood**, the most basic material for building structures and tools.
- **Function**:
    - **Produces Wood** over time.
    - **Requires Workers** to operate.

### **Stone Quarry**
- **Purpose**: The **Stone Quarry** gathers **stone**, a vital material for constructing more advanced buildings.
- **Function**:
    - **Produces Stone** over time.
    - **Requires Workers** to mine.

### **Marble Quarry**
- **Purpose**: The **Marble Quarry** provides **marble**, an advanced material needed for more complex and prestigious structures.
- **Function**:
    - **Produces Marble** over time.
    - **Requires Workers** to operate.

### **Bronze Mine**
- **Purpose**: The **Bronze Mine** is where **bronze** is extracted, a critical resource for making **bronze tools**.
- **Function**:
    - **Produces Bronze** over time.
    - **Requires Workers** to operate.

### **Smith**
- **Purpose**: The **Smith** crafts **bronze tools** from **bronze** and **wood**, which are required for advanced construction.
- **Function**:
    - **Converts Bronze and Wood** into **Bronze Tools**.
    - **Requires Workers** to operate.

### **Temple**
- **Purpose**: The **Temple** is a prestigious building and demonstrate the prosperity of the city.
- **Function**:
    - **Requires Marble**, **Stone** and **Bronze Tools** to build.
    - **Increases Prestige** and **Happiness**.
    - **Upgradable** to improve its effects on the city.

---