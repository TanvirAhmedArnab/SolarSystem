# Solar System Simulation

## Living Game Design Document

**Project:** Solar System Simulation - Unity Portfolio Build  
**Author and product owner:** Tanvir  
**Document owner:** Tanvir  
**Document status:** Draft with owner decisions recorded  
**Version:** 0.4.0  
**Last updated:** 2026-07-22  
**Unity baseline:** Unity 6000.5.3f1, Universal Render Pipeline 17.5.0  
**Repository:** `C:\Users\taarn\Desktop\Unity\SolarSystem`

> **Living-document rule:** This GDD is the authoritative description of the intended experience and its product-level acceptance criteria. Technical implementation belongs in the TDD. Art production specifications belong in the Art Bible. Proposed decisions are not approved until the product owner changes their status.

## 1. Document Control

### 1.1 Status vocabulary

- **[APPROVED]** - accepted by the product owner and safe to plan against.
- **[PROPOSED]** - recommended working direction awaiting owner approval.
- **[OPEN]** - requires a decision or evidence before commitment.
- **[DEFERRED]** - intentionally excluded from the current milestone.
- **[REJECTED]** - considered and explicitly declined, retained for auditability.

### 1.2 Revision history

| Version | Date | Author | Summary | Approval |
|---|---|---|---|---|
| 0.1.0 | 2026-07-22 | Codex, for Tanvir | First structured draft derived from the project brief and supplied research plan | Pending owner review |
| 0.2.0 | 2026-07-22 | Codex, for Tanvir | Recorded owner decisions for scientific scope, scale experience, input, audio, deliverables, target hardware category, and moon scope | Approved decisions incorporated; schedule pending clarification |
| 0.3.0 | 2026-07-22 | Codex, for Tanvir | Preserved the complete portfolio scope and converted July 29 into an initial inspection and planning milestone | Schedule strategy approved; final release date pending project inspection |
| 0.4.0 | 2026-07-22 | Codex, for Tanvir | Recorded approval of the GDD authority, URP, deterministic analytical orbits, and lightweight trunk-based development | Foundation decisions approved |

### 1.3 Source hierarchy

When sources disagree, use this order:

1. Explicit, dated product-owner decisions recorded in this document.
2. Verified primary scientific sources, including NASA and USGS datasets.
3. The approved TDD and Art Bible for implementation and presentation details.
4. The supplied *Unity Solar System Simulation Plan* as research input.
5. Tutorials and community sources as non-authoritative implementation references.

Scientific values, asset licenses, and technical claims must be verified before they become release criteria.

## 2. Executive Summary

Solar System Simulation is an interactive real-time visualization of the Sun, eight planets, and selected major moons. Its deliberately narrow interaction scope supports a high level of visual polish, scientific transparency, engineering quality, and portfolio presentation.

The experience allows a visitor to explore the system freely, focus on a celestial body, observe time-scaled motion, compare readable presentation scale with a more physically representative scale, and inspect concise factual overlays. The project is not intended to reproduce every gravitational interaction or function as a research-grade ephemeris.

### 2.1 Portfolio thesis

**[PROPOSED]** The project demonstrates that Tanvir can turn extreme-scale scientific data into an accessible, performant, visually compelling Unity experience using clean architecture, deterministic simulation, custom rendering, professional UX, automated validation, and auditable documentation.

### 2.2 Experience promise

> Explore the Solar System as a coherent moving system, move seamlessly from planetary overview to close inspection, and understand what has been scaled or simplified without losing a sense of astronomical wonder.

## 3. Vision and Design Pillars

### 3.1 Scientific integrity

The simulation clearly distinguishes verified source data from presentation transformations. It must never imply that exaggerated radii, compressed distances, or accelerated time are literal.

### 3.2 Legibility across extreme scale

Planets, moons, orbit paths, labels, and transitions remain understandable from system-wide and close-focus views. Scale changes should teach the scale problem rather than hide it.

### 3.3 Cinematic restraint

Lighting, bloom, atmosphere, motion, and UI support wonder and clarity. Effects must not obscure silhouettes, surface detail, labels, or the scientific framing.

### 3.4 Engineering credibility

Deterministic data-driven behavior, separation of concerns, tests, profiling evidence, and clear documentation are visible parts of the portfolio story.

### 3.5 Frictionless exploration

A first-time visitor should understand how to navigate, focus a body, change time, and identify scale mode without reading a manual.

## 4. Audience and Use Context

### 4.1 Primary audience

- Unity and gameplay engineering recruiters or hiring managers.
- Technical artists and developers evaluating real-time graphics work.
- Portfolio visitors with a general interest in space.

### 4.2 Secondary audience

- Students and hobbyists interested in an approachable Solar System visualization.
- Developers studying clean Unity project structure and data-driven simulation.

### 4.3 Intended session

**[PROPOSED]** A satisfying first session lasts 5-10 minutes. Within the first 60 seconds, a visitor can focus a planet, move the camera, and alter simulation speed. A guided cinematic route can communicate the project in a 60-90 second portfolio video.

## 5. Product Scope

### 5.1 Minimum portfolio release

The first public portfolio release includes:

- The Sun and all eight planets.
- Earth's Moon and a curated set of visually or scientifically meaningful major moons.
- Data-driven axial rotation and analytical orbital motion.
- Readable presentation scale and a comparative physical-scale mode.
- Pause, resume, and adjustable simulation speed.
- Free-flight navigation, body focus, and a cinematic tour.
- Orbit visualization with clear visibility controls.
- Selection and information overlays for celestial bodies.
- Physically based planet materials appropriate to URP.
- Solar emission, restrained bloom, exposure control, and atmospheric effects where relevant.
- Saturn's ring system; Uranus's rings if quality and schedule allow.
- Professional onboarding, settings, credits, source attribution, and accessibility essentials.
- A stable desktop build and portfolio-ready repository documentation.

### 5.2 Candidate moon set

**[APPROVED]** Minimum moon set: Earth's Moon, Io, Europa, Ganymede, Callisto, Titan, and Triton. The set provides recognizable bodies, varied visuals, nested orbits, and retrograde-motion coverage without attempting exhaustive satellite content.

### 5.3 Stretch scope

These features require separate approval after the minimum release meets its quality bar:

- **[DEFERRED]** Asteroid belt with density-aware instancing.
- **[DEFERRED]** Dwarf planets, beginning with Pluto and Charon.
- **[DEFERRED]** Comets and reusable pooled transient-body behavior.
- **[DEFERRED]** Educational guided lessons or quizzes.
- **[DEFERRED]** VR, mobile, console, or WebGL releases.
- **[DEFERRED]** Full N-body gravity, spacecraft, collisions, or procedural galaxy content.

### 5.4 Explicit non-goals

- Research-grade ephemeris prediction.
- A complete catalog of moons, asteroids, and minor bodies.
- Built-in Rigidbody gravity as the orbital solution.
- Gameplay systems such as resource collection, combat, crafting, or colonization.
- Photorealism at the expense of readability, performance, or truthful scale communication.

## 6. Core Experience

### 6.1 Experience loop

1. **Orient:** Enter a composed system overview with clear scale and time status.
2. **Explore:** Fly freely, orbit the current focus, or select a labeled body.
3. **Inspect:** Transition smoothly to a safe close view and reveal concise body data.
4. **Compare:** Change time speed, orbit visibility, or scale mode and observe the result.
5. **Discover:** Follow a cinematic route or select another body to continue exploring.

### 6.2 First-run experience

**[PROPOSED]** The first launch opens in presentation scale with simulation running at a calm, readable speed. A minimal contextual overlay introduces movement, selection, focus, and time controls. Prompts disappear after successful use and remain available from Help.

### 6.3 Interaction states

- **System overview:** broad navigation and system composition.
- **Free flight:** six-degree movement with speed modulation and safe defaults.
- **Focus orbit:** camera orbits a selected body with bounded zoom.
- **Cinematic tour:** authored camera sequence with skippable transitions and optional captions.
- **UI interaction:** cursor-focused state that does not also move the camera.
- **Paused simulation:** camera and UI remain usable while celestial motion stops.

## 7. Simulation Design

### 7.1 Simulation model

**[APPROVED]** Use an educational-visualization accuracy tier: verified planetary data and convincing deterministic elliptical motion, without claiming exact real-world positions for a selected date. Analytical orbits are driven by a simulation clock and support orbital inclination where reliable elements are available. The simulation does not use Rigidbody forces for orbital motion.

This choice produces more credible motion than circular transform rotation while remaining testable, performant, and appropriately scoped. The TDD will define formulas, epochs, numerical types, coordinate spaces, and precision strategy.

### 7.2 Scale modes

#### Presentation scale

**[PROPOSED]** The default mode uses monotonic distance compression and controlled radius exaggeration. Ordering and parent-child relationships remain correct, while the UI clearly labels the mode as visually adjusted.

#### Physical comparison scale

**[APPROVED]** Physical scale is presented through a controlled guided comparison explaining why realistic distances and body sizes are difficult to view together. It communicates relative physical scale as faithfully as the rendering architecture allows without claiming that raw astronomical coordinates are rendered directly in single-precision Unity transforms. Free navigation in this mode is not required for the minimum release.

#### Transition behavior

- Transitions are smooth, cancellable, and deterministic.
- Camera framing remains valid throughout the transition.
- Labels, orbit paths, and focus targets do not visibly detach from their bodies.
- The interface always identifies the active scale mode.

### 7.3 Simulation time

**[PROPOSED]** Controls include pause and the following labeled multipliers: `1x`, `10x`, `100x`, `1,000x`, and `10,000x`, with meanings defined relative to a documented baseline simulation rate. A scrubber is excluded from the first release unless backward-time behavior is designed and tested.

### 7.4 Initial conditions and determinism

- A documented epoch or intentionally authored showcase state initializes all bodies.
- Random phase assignment is not permitted in portfolio builds.
- Given the same data, epoch, and simulation time, the system produces the same state.
- Save-state persistence is **[OPEN]** and not required for the minimum release.

### 7.5 Scientific transparency

The application credits its scientific sources and exposes a concise explanation of:

- Which radii, distances, periods, tilts, and orbital elements are source-derived.
- Which visual scales and time rates are transformed.
- Which physical effects are simplified or omitted.
- The date and version of the validated dataset.

## 8. Celestial Content

### 8.1 Required bodies

| Group | Bodies | Release status |
|---|---|---|
| Star | Sun | Required |
| Terrestrial planets | Mercury, Venus, Earth, Mars | Required |
| Giant planets | Jupiter, Saturn, Uranus, Neptune | Required |
| Major moons | Moon, Io, Europa, Ganymede, Callisto, Titan, Triton | Proposed minimum |

### 8.2 Body information overlay

Each selectable body provides, where applicable and source-verified:

- Display name and body classification.
- Mean radius.
- Current distance from its parent in simulation units and a meaningful scientific unit.
- Rotation period and orbital period.
- Current display speed or simulation-time rate.
- One short curated fact.
- Source or credits access without cluttering the primary overlay.

Values must include units and avoid unjustified precision.

### 8.3 Selection behavior

- A visible body, label, or navigator entry can select the same target.
- Selection is clearly differentiated from camera focus.
- Focus transitions can be cancelled or redirected.
- Occluded or extremely distant bodies remain reachable through the navigator.
- The selected state remains synchronized across world labels, navigator, and detail panel.

## 9. Camera Design

### 9.1 Camera goals

- Preserve orientation across enormous changes in visual scale.
- Avoid clipping through body surfaces and rings.
- Make focus transitions smooth without inducing motion discomfort.
- Provide capture-friendly compositions for screenshots and video.

### 9.2 Free-flight mode

**[APPROVED]** Keyboard and mouse are the supported input scheme for the first public release. Movement speed scales by context and supports a temporary boost. Acceleration and deceleration are damped, with a reset or return-to-focus action always available. Gamepad support is deferred unless separately approved.

### 9.3 Focus mode

- Orbit, pan where appropriate, and zoom around the selected body.
- Minimum distance accounts for atmosphere, rings, and presentation radius.
- Target motion does not introduce visible camera jitter.
- Changing target uses a staged transition that avoids crossing through the Sun or planets.

### 9.4 Cinematic mode

**[PROPOSED]** A curated, skippable tour visits the Sun, Earth-Moon system, Jupiter and its major moons, Saturn's rings, and the outer planets. The tour uses the same simulation state and camera services as interactive modes rather than a duplicate scene.

## 10. User Interface and Information Design

### 10.1 HUD hierarchy

- Current selected or focused body.
- Simulation state: running or paused, time multiplier, and scale mode.
- Contextual body information.
- Optional world-space labels and orbit-path controls.
- Access to navigator, settings, help, credits, and sources.

### 10.2 Visual direction

**[PROPOSED]** Use a restrained observatory-inspired interface: dark translucent surfaces, high-contrast neutral text, a cool accent color, and limited warm highlights for active or warning states. UI must remain legible over bright planets and black space.

### 10.3 Onboarding and help

- Teach controls contextually and briefly.
- Allow tutorials to be reopened.
- Display input-specific prompts from the active control scheme.
- Avoid unexplained astronomy or graphics terminology.

### 10.4 Accessibility baseline

**[PROPOSED]** Minimum requirements:

- UI scaling and readable minimum text sizes.
- Full keyboard operation for primary UI flows.
- Rebindable gameplay controls where supported by the Input System.
- Reduced camera motion or instant-transition option.
- Independent bloom and motion-blur controls; motion blur defaults off.
- Do not use color as the sole indicator of selection or state.
- Captions for any narrated cinematic content.

## 11. Visual and Art Direction

### 11.1 Rendering direction

**[APPROVED]** Continue with URP. It is already configured, supports the target visual techniques, and offers an appropriate balance of custom rendering capability, performance, portability, and portfolio maintainability. Changing to HDRP would require a separately approved pipeline migration.

### 11.2 Planet presentation

- Surface materials use physically based inputs where source assets support them.
- Texture resolution is selected by observed screen-space need and memory budget, not maximum availability.
- Earth may use independent surface, cloud, atmosphere, and night-light treatments.
- Atmospheres use body-specific parameters and avoid a uniform colored-shell appearance.
- Rings preserve radial structure, transparency, lighting readability, and correct orientation.
- The Sun reads as emissive and energetic without destroying exposure across the scene.

### 11.3 Lighting and post-processing

- The Sun is the dominant motivated light source.
- Ambient fill may preserve silhouettes but must not flatten day-night separation.
- Bloom and exposure remain stable during focus transitions.
- Color grading is restrained and consistent.
- Vignette is subtle or disabled when it interferes with UI and capture work.
- Motion blur defaults off unless testing proves it improves the experience without discomfort.

### 11.4 Asset provenance

Every external asset requires a recorded source URL, creator or organization, license, attribution text, download date, and any modification notes. “Free” is not an acceptable license description. Unverified assets cannot enter a public build.

## 12. Audio Direction

**[APPROVED]** The minimum release uses a subtle ambient score and restrained interface audio. Narration is not required.

If audio is included:

- It supports contemplation and interaction clarity rather than implying sound travels through space.
- UI, transition, and ambience levels are independently adjustable.
- All sources and licenses are recorded.
- The application starts at a considerate volume and supports mute.

## 13. Technical Product Constraints

These constraints define product quality; implementation details belong in the TDD.

- Runtime and editor code remain separated.
- Celestial definitions are data-driven, with ScriptableObjects as the authoring interface.
- Simulation, presentation transforms, rendering, camera, input, and UI have distinct responsibilities.
- Public APIs use XML documentation where it improves consumer understanding.
- Events or narrow interfaces communicate state changes without hidden scene-wide dependencies.
- Runtime discovery calls and global mutable singletons are not default architecture choices.
- Scene and build behavior is deterministic and testable.
- No built-in physics simulation is used to maintain orbits.
- Dynamic populations such as future comets use pooling only when introduced and justified.

## 14. Performance and Quality Targets

### 14.1 Target platform

**[APPROVED]** First release target: Windows 10/11, x86-64 desktop, keyboard and mouse, 1920x1080 reference resolution.

**[APPROVED]** Use a reasonable mid-range gaming PC as the performance category. The exact representative CPU, GPU, and RAM configuration will be recorded in the TDD or performance test plan before formal profiling begins.

### 14.2 Runtime targets

**[PROPOSED]** On the selected representative mid-range gaming PC:

- 60 frames per second at 1920x1080 during normal exploration.
- No sustained frame-time spikes during focus or scale transitions.
- No visible transform jitter in supported camera ranges.
- No avoidable per-frame managed allocations during steady-state simulation.
- Initial load and peak memory targets are established after the first representative art vertical slice.

### 14.3 Visual quality bar

- No visible seams, inverted normals, broken transparency sorting, or detached atmosphere layers in intended views.
- No unreadable labels or UI collisions at supported resolutions and UI scales.
- No uncontrolled exposure shifts when changing focus.
- Planet surfaces and rings remain stable during camera motion.

## 15. Validation and Acceptance

### 15.1 Functional acceptance

- Every required body loads from validated data and can be selected.
- Orbits and rotations remain deterministic across repeated runs.
- Pause and time-scale controls affect all simulation-driven systems consistently.
- Scale-mode transitions complete and can safely handle interruption.
- Camera modes preserve valid focus and never strand the user.
- UI values update correctly and always show units.

### 15.2 Scientific acceptance

- Required source values are checked against recorded primary references.
- Retrograde rotation and orbit cases are represented intentionally.
- Presentation transformations are documented and labeled.
- Automated tests cover representative calculations, boundary cases, and deterministic state evaluation.

### 15.3 Portfolio acceptance

- Repository opens from documented setup steps without undocumented local dependencies.
- README explains the problem, experience, architecture, verification, and asset licensing.
- Screenshots and a short video demonstrate system scale, close-focus detail, UI, and cinematic framing.
- The approved release package includes a public GitHub repository, downloadable Windows build, recorded portfolio video, screenshots and animated clips, and a personal portfolio page or case study.
- Code samples shown publicly represent the shipping architecture, not throwaway prototypes.
- Known limitations are stated honestly.

## 16. Documentation Deliverables

The project documentation set will include:

- Living GDD.
- Technical Design Document.
- Art Bible.
- Project Setup Guide.
- Coding Standards Guide.
- Commit Message Guidelines.
- Portfolio-ready `README.md`.
- `CHANGELOG.md` following semantic versioning conventions.
- License and third-party attribution records.
- Architecture and class diagrams where they clarify the system.
- Validation, profiling, and final-polish checklists.

Each rule should have one authoritative owner document; other documents link to it rather than duplicating it.

## 17. Repository and Delivery Strategy

### 17.1 Version control

- Unity-aware `.gitignore` and `.gitattributes` remain committed.
- Large binary assets use Git LFS when repository policy requires it.
- Generated Unity folders, local editor state, temporary files, and secrets are never committed.
- Scene, prefab, asset, package, and ProjectSettings diffs receive heightened review.
- Conventional Commits and semantic version tags document public progress.
- No commit is made without Tanvir's explicit approval.

### 17.2 Branching

**[APPROVED]** Use lightweight trunk-based development: short-lived feature branches, small reviewed changes, and a protected `main` branch when hosted. This fits a solo portfolio project better than GitFlow's long-lived integration and release branches while still demonstrating professional review discipline.

## 18. Milestones

### Milestone 0 - Foundation and decisions

- Approve vision, minimum scope, target platform, and scientific-accuracy policy.
- Establish documentation structure and repository health baseline.
- Validate data and asset-license recording workflows.
- Inspect the existing Unity project, assets, packages, settings, and repository health deeply enough to estimate the remaining work.
- Produce a realistic milestone schedule for the complete portfolio release.

**[APPROVED] Initial milestone target:** 2026-07-29. This is a foundation, inspection, risk-reduction, and scheduling milestone; it is not the final portfolio-release deadline.

### Milestone 1 - Simulation graybox

- Implement deterministic clock, body data, orbit evaluation, scale transformation, and automated tests.
- Demonstrate the Sun, Earth, Moon, and one outer planet in graybox form.
- Validate precision and camera-range strategy before broad content production.

### Milestone 2 - Interaction vertical slice

- Deliver system overview, selection, focus camera, time controls, scale controls, and representative UI.
- Confirm the first-minute experience with an external observer if possible.

### Milestone 3 - Visual vertical slice

- Bring the Sun, Earth-Moon system, Jupiter, and Saturn to representative visual quality.
- Establish the Art Bible and measured performance budget.

### Milestone 4 - Content completion

- Populate all approved bodies, materials, labels, facts, sources, and credits.
- Complete cinematic route and accessibility baseline.

### Milestone 5 - Portfolio release

- Complete optimization, testing, repository audit, build verification, screenshots, video, README, changelog, licensing review, and known-limitations record.

**[APPROVED] Schedule strategy:** Retain the complete approved portfolio scope. Treat 2026-07-29 as the initial milestone rather than the final deadline. Establish the remaining milestone dates and final release target after inspecting the project and estimating the work against the quality bar. The earlier 10-hour expectation is not a binding cap on the full release.

## 19. Risks and Mitigations

### Extreme-scale precision

**Risk:** Jitter, clipping, Z-fighting, or unusable camera controls across system and body scales.  
**Mitigation:** Validate coordinate and rendering strategy in Milestone 1 before final content work; keep physical data separate from render transforms.

### Visual ambition exceeding scope

**Risk:** Atmospheres, rings, high-resolution textures, and cinematic polish delay completion.  
**Mitigation:** Establish representative visual slices, explicit budgets, and minimum body treatments before scaling content production.

### Scientific overclaiming

**Risk:** Approximate motion or transformed scale is presented as physically exact.  
**Mitigation:** Maintain source records, defined accuracy tiers, visible mode labels, deterministic tests, and an honest limitations section.

### Asset licensing uncertainty

**Risk:** A texture described as free has incompatible attribution or redistribution terms.  
**Mitigation:** Require license verification and provenance records before public inclusion; retain source and modification information.

### Repository noise

**Risk:** Generated files, large textures, or unrelated settings changes obscure portfolio-quality history.  
**Mitigation:** Use preflight checks, Git LFS policy, narrow commits, and explicit review of high-risk Unity files.

## 20. Open Decisions

No owner-level product decisions are currently open from the first GDD review. Project inspection may identify new decisions; any such decisions must be added here with an owner and milestone gate rather than being assumed silently.



## 21. Decision Log

Record owner decisions here at the time they are made. Link to a separate ADR or TDD section when technical rationale is extensive.

| ID | Date | Decision | Status | Owner |
|---|---|---|---|---|
| GDD-001 | 2026-07-22 | Treat this document as the living product-design authority; technical detail remains in the TDD | Approved | Tanvir |
| GDD-002 | 2026-07-22 | Continue with URP rather than migrate to HDRP | Approved | Tanvir |
| GDD-003 | 2026-07-22 | Use deterministic analytical orbits rather than Rigidbody orbital physics | Approved | Tanvir |
| GDD-004 | 2026-07-22 | Use lightweight trunk-based development | Approved | Tanvir |
| GDD-005 | 2026-07-22 | Target a reasonable mid-range gaming PC at 1080p, with the exact reference configuration documented before profiling | Approved | Tanvir |
| GDD-006 | 2026-07-22 | Use educational-visualization accuracy with verified data and convincing elliptical motion, without date-exact ephemeris claims | Approved | Tanvir |
| GDD-007 | 2026-07-22 | Present physical scale through a controlled guided comparison rather than require free navigation | Approved | Tanvir |
| GDD-008 | 2026-07-22 | Support keyboard and mouse for the first public release | Approved | Tanvir |
| GDD-009 | 2026-07-22 | Include ambient music and restrained UI sounds; narration is not required | Approved | Tanvir |
| GDD-010 | 2026-07-22 | Require a public GitHub repository, Windows build, portfolio video, screenshots and clips, and a portfolio case study | Approved | Tanvir |
| GDD-011 | 2026-07-22 | Include the Moon, Io, Europa, Ganymede, Callisto, Titan, and Triton in the minimum moon set | Approved | Tanvir |
| GDD-012 | 2026-07-22 | Treat 2026-07-29 as the desired date, pending a decision between a 10-hour vertical slice and the full release scope | Superseded by GDD-013 | Tanvir |
| GDD-013 | 2026-07-22 | Retain the complete portfolio-release scope, use 2026-07-29 as the initial inspection and planning milestone, and set the final schedule after project inspection | Approved | Tanvir |

## 22. Definition of Done for GDD Version 1.0

The GDD reaches version 1.0 when:

- Tanvir has reviewed and approved the vision, pillars, minimum scope, and non-goals.
- All decisions that gate Milestones 1-3 are approved or deliberately deferred.
- Each product requirement has an observable acceptance condition.
- GDD, TDD, Art Bible, and setup documentation have clear ownership boundaries.
- Scientific and asset-source policies are agreed.
- Revision history and decision log reflect the review outcome.
- The document contains no unmarked assumptions presented as approved facts.
